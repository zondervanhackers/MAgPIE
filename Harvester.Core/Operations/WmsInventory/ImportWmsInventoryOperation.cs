using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.SharedLibrary.Parsing;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsInventory
{
    /// <inheritdoc />
    public class ImportWmsInventoryOperation : OperationBase
    {
        private readonly RepositoryArgumentsBase _databaseArgs;
        private readonly RepositoryArgumentsBase _harvesterArgs;
        private readonly RepositoryArgumentsBase _directoryArgs;

        public ImportWmsInventoryOperation(RepositoryArgumentsBase HarvesterDatabase, RepositoryArgumentsBase DestinationDatabase, RepositoryArgumentsBase SourceDirectory)
        {
            Contract.Assert(DestinationDatabase != null, "Destination Database was null");
            Contract.Assert(HarvesterDatabase != null, "Havester Database was null");
            Contract.Assert(SourceDirectory != null, "Source Directory was null");

            _databaseArgs = DestinationDatabase;
            _harvesterArgs = HarvesterDatabase;
            _directoryArgs = SourceDirectory;
        }

        public override void Execute(DateTime runDate, Action<string> logMessage, CancellationToken cancellationToken)
        {
            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(_directoryArgs))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                Regex filePattern = new Regex(@"^((ITU[.]Item_inventories[.]\d{8})|(ITU[.]Circulation_Item_Inventories[.]\d{8}))[.]txt([.]zip)?$", RegexOptions.IgnoreCase);

                var sourceFilesBeforeZip = source.ListFiles().Where(y => filePattern.IsMatch(y.Name)).ToArray();
                foreach (DirectoryObjectMetadata zipFile in sourceFilesBeforeZip.Where(x => x.Path.Contains(".txt.zip")))
                {
                    if (sourceFilesBeforeZip.All(x => x.Name != zipFile.Name.Replace(".zip", "")))
                        ZipFile.ExtractToDirectory(zipFile.Path, Path.GetDirectoryName(zipFile.Path.Replace(".txt.zip", "")));
                    //source.DeleteFile(zipFile.Path);
                }

                List<String> modified = new List<String>();
                Int32 newCount = 0;

                List<DirectoryObjectMetadata> sourceFiles = source.ListFiles("/").Where(y => filePattern.IsMatch(y.Name) && !y.Name.EndsWith(".zip")).ToList();

                using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(_harvesterArgs))
                {
                    logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                    Entities.Repository repository = harvester.DataContext.Repositories.First(y => y.Name == source.Name);

                    if (OperationID == 0)
                    {
                        logMessage("Warning: OperationID was not set properly. Correcting this.");
                        OperationID = harvester.DataContext.Operations.First(d => d.Name == Name).ID;
                    }
                    
                    Dictionary<String, DirectoryRecord> dictionary = harvester.DataContext.DirectoryRecords.Where(d => d.OperationID == OperationID && d.RepositoryID == repository.ID).ToDictionary(d => d.FilePath);

                    foreach (DirectoryObjectMetadata file in sourceFiles)
                    {
                        if (!dictionary.ContainsKey(file.Path))
                        {
                            modified.Add(file.Name);
                            newCount++;
                        }
                        else
                        {
                            DirectoryRecord element = dictionary[file.Path];

                            if (file.ModifiedDate > element.FileModifiedDate)
                            {
                                modified.Add(file.Name);
                            }
                        }
                    }
                }

                if (modified.Count == 0 && newCount == 0)
                {
                    logMessage("No Records to be processed.");
                    return;
                }

                logMessage($"Discovered {modified.Count} files to be processed ({newCount} new and {modified.Count - newCount} updated).");

                if (cancellationToken.IsCancellationRequested)
                {
                    source.Dispose();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(_databaseArgs))
                {
                    logMessage($"Connected to destination database '{destination.Name}' ({destination.ConnectionString})");
                    bool exceptionHandled = false;
                    List<DirectoryObjectMetadata> successfulFiles = new List<DirectoryObjectMetadata>();

                    StreamParser<WmsInventoryRecord> Parser = new StreamParser<WmsInventoryRecord>();
                    StreamParser<WmsInventoryRecordEdge> ParserEdge = new StreamParser<WmsInventoryRecordEdge>();
                    StreamParser<WmsInventoryRecordDiff> ParserDiff = new StreamParser<WmsInventoryRecordDiff>();
                    StreamParser<WmsInventoryRecord2018> Parser2018 = new StreamParser<WmsInventoryRecord2018>();

                    if (cancellationToken.IsCancellationRequested)
                    {
                        source.Dispose();
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    var completedFiles = modified.Select<string, (string file, DateTime RunDate, IEnumerable<IWmsInventoryRecord> records)>(file =>
                    {
                        logMessage($"Processing '{file}':");

                        Stream inputStream = source.OpenFile(file);
                        DateTime fileRunDate = GetFileDate(file);
                        string headers = ExtractHeader(inputStream);

                        //logMessage(headers);

                        if (IsWmsInventoryRecord(headers))
                            return (file, fileRunDate, Parser.ParseStream(inputStream));

                        if (IsWmsInventoryRecord2018(headers))
                            return (file, fileRunDate, Parser2018.ParseStream(inputStream));

                        if (IsWmsInventoryRecordDiff(headers))
                            return (file, fileRunDate, ParserDiff.ParseStream(inputStream));

                        if (IsWmsInventoryRecordEdge(headers))
                            return (file, fileRunDate, ParserEdge.ParseStream(inputStream));

                        throw new InvalidDataException($"Header format not recognized: '{headers}'");

                    }).Select(wmsFileRecord =>
                    {
                        var parsed = wmsFileRecord.records.Select(wms => new InventoryRecord
                        {
                            OclcNumber = wms.OclcNumber,
                            Title = ParseTitle(wms.Title),
                            Author = wms.Author,
                            MaterialFormat = wms.MaterialFormat.ToString(),
                            Barcode = wms.Barcode,
                            Cost = wms.Cost,
                            LastInventoriedDate = wms.LastInventoriedDate,
                            DeletedDate = wms.ItemDeletedDate,
                            ItemType = wms.ItemType.ToString(),
                            CallNumber = wms.CallNumber,
                            ShelvingLocation = wms.ShelvingLocation?.ToString(),
                            CurrentStatus = wms.CurrentStatus?.ToString(),
                            Description = wms.Description,
                            RunDate = wmsFileRecord.RunDate,
                            Anomalous = AnomalousBarcode(wms.Barcode),
                        }).Where(y => y.Title != null || y.Barcode != null).GroupBy(x => new { x.OclcNumber, x.Barcode, x.RunDate }).Select(x => x.First()).ToArray();

                        logMessage($"Records Found: {parsed.Length}");

                        if (cancellationToken.IsCancellationRequested)
                        {
                            logMessage("Operation was cancelled");
                            exceptionHandled = true;
                            return null;
                        }

                        if (parsed.Length <= 0)
                        {
                            logMessage("Failed to parse properly and return any meaningful records. This might mean that non of the parsed records had a Title or Barcode.");
                            exceptionHandled = true;
                            return null;
                        }

                        try
                        {
                            destination.DataContext.BulkImportInventory(
                                parsed.ToDataReader(r => new object[] { r.OclcNumber, r.Title, r.MaterialFormat, r.Author, r.Barcode, r.Cost, r.LastInventoriedDate, r.DeletedDate, r.ItemType, r.CallNumber, r.ShelvingLocation, r.CurrentStatus, r.Description, r.RunDate, r.Anomalous }));
                        }

                        catch(SqlException ex)
                        {
                            logMessage(ex.Message);
                            if (ex.InnerException != null)
                                logMessage(ex.InnerException.ToString());
                            logMessage("Sql Server was most likely put into an unusable state after this exception and thus the whole operation was canceled.");
                            exceptionHandled = true;
                        }

                        return sourceFiles.First(x => x.Name == wmsFileRecord.file);
                    }).Where(x => x != null);

                    foreach (var success in completedFiles)
                    {
                        successfulFiles.Add(success);
                    }

                    UpdateHarvesterRecord(logMessage, successfulFiles, source.Name, _harvesterArgs);

                    if (exceptionHandled)
                    {
                        destination.DataContext.Connection.Close();
                        destination.DataContext.Dispose();
                        throw new Exception("An Exception was encountered. At least one file failed");
                    }
                }
            }
        }

        private static bool IsWmsInventoryRecord2018(string fileHeader)
        {
            string header = "Institution_Symbol|Item_Holding_Location|Item_Permanent_Shelving_Location|Item_Temporary_Shelving_Location|Item_Type|Item_Call_Number|Item_Enumeration_and_Chronology|Author_Name|Title|LHR_Item_Materials_Specified|Material_Format|OCLC_Number|Title_ISBN|Publication_Date|Item_Barcode|LHR_Item_Cost|LHR_Item_Nonpublic_Note|LHR_Item_Public_Note|Item_Status_Current_Status|Item_Due_Date|Item_Issued_Count|Issued_Count_YTD|Item_Soft_Issued_Count|Item_Soft_Issued_Count_YTD|Item_Last_Issued_Date|Item_Last_Inventoried_Date|Item_Deleted_Date|LHR_Date_Entered_on_File|LHR_Item_Acquired_Date|Language_Code";
            return fileHeader == header || String.Equals(fileHeader, header, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool IsWmsInventoryRecordEdge(string fileHeader)
        {
            string header = "Inst_Symbol|Holding Location|Shelving Location|Temp Shelving Location|Item Type|Call Number|Author|Title or Description|Material Format|OCLC Number|ISBN|Item Barcode|Cost|Staff Note|Public Note|Current Status|Loan Date Due|Issued Count|Issued Count YTD|Last Issued Date|Last Inventoried Date|Item Deleted Date";
            return fileHeader == header || String.Equals(fileHeader, header, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool IsWmsInventoryRecord(string fileHeader)
        {
            string header = "Inst_symbol|BarCode|Oclc_number|Title|Material_format|Author|Call Number|Description|Shelving Location|Holding Location|Cost|Temporary_Shelving_Location|Current_Status|Loan_Date_Due|Item_Deleted_date|Last_inventoried_date|Issued_count|Issued_count_ytd|Last_issued_date|Item Type|Staff Note|Public Note|ISBN";
            return fileHeader == header || fileHeader.ToLower() == header.ToLower();
        }

        private static bool IsWmsInventoryRecordDiff(string fileHeader)
        {
            string header = "Inst_Symbol|Holding Location|Shelving Location|Temp Shelving Location|Item Type|Call Number|Author|Title or Description|Textual Holdings|Material Format|OCLC Number|ISBN|Item Barcode|Cost|Staff Note|Public Note|Current Status|Loan Date Due|Issued Count|Issued Count YTD|Last Issued Date|Last Inventoried Date|Item Deleted Date";
            return fileHeader == header || fileHeader.ToLower() == header.ToLower();
        }

        private static string ExtractHeader(Stream inputStream)
        {
            using (var streamReader = new StreamReader(inputStream, Encoding.UTF8, true, 1024, true))
            {
                return streamReader.ReadLine();
            }
        }

        private static bool AnomalousBarcode(string barcode)
        {
            if (barcode == null)
                return true;

            return barcode.Length != 14 || barcode.Any(Char.IsLetter);
        }

        private static DateTime GetFileDate(string filename)
        {
            string dateString = filename.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[2];
            DateTime RunDate = DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            return RunDate;
        }

        private static string ParseTitle(string title)
        {
            return title == null 
                ? null : title.Contains('/')
                ? title.Substring(0, title.IndexOf('/')).Trim() : title.Trim();
        }
    }
}