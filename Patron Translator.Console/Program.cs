using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ZondervanLibrary.SharedLibrary.Collections;

using ZondervanLibrary.PatronTranslator.Console.IO;
using ZondervanLibrary.PatronTranslator.Console.Repository;
using ZondervanLibrary.PatronTranslator.Console.Patrons;

namespace ZondervanLibrary.PatronTranslator.Console
{
    enum Destination
    {
        Production,
        Test
    }

    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Zondervan Library Patron Translation Utility.");
            System.Console.WriteLine();

            if (args.Length != 1)
            {
                System.Console.WriteLine("Error: Expected argument of either 'Production' or 'Test'");
                return;
            }

            Destination destination;

            switch (args[0])
            {
                case "Production":
                    destination = Destination.Production;
                    break;
                case "Test":
                    destination = Destination.Test;
                    break;
                default:
                    System.Console.WriteLine("Error: Expected argument of either 'Production' or 'Test'");
                    return;
            }

            String[] lisFiles = Directory.GetFiles("./", "*.lis");

            if (lisFiles.Length != 1)
            {
                System.Console.WriteLine("Error: Expected exactly one *.lis file in the directory to convert.");
                return;
            }

            String[] xmlFiles = Directory.GetFiles("./", "*-*-*.xml");

            if (xmlFiles.Length > 1)
            {
                System.Console.WriteLine("Error: Expected exactly zero or one *.xml file in the directory to compare to.");
                return;
            }

            // Load existing *.lis file and convert to persona
            System.Console.Write("Converting {0}: ", lisFiles[0]);

            // Convert patrons to personas
            FileStreamFactory newFileStreamFactory = new FileStreamFactory(lisFiles[0]);
            IRepository<Patron> newRepository = new FlatRepository<Patron>(newFileStreamFactory);
            IConverter<Patron, Persona> converter = new PatronToPersonaConverter(DateTime.Now);
            IEnumerable<Persona> newPersonas = newRepository.AsQueryable().Select(patron => converter.Convert(patron));

            System.Console.WriteLine("{0} records converted.", newPersonas.Count());
            System.Console.WriteLine();

            // Do comparison if old personas file present
            if (xmlFiles.Length == 1)
            {
                // Load old persona repo to diffentiate against
                FileStreamFactory oldStreamFactory = new FileStreamFactory(xmlFiles[0]);
                IRepository<Persona> oldRepository = new XmlRepository<Persona, OclcPersonas>(oldStreamFactory);
                IEnumerable<Persona> oldPersonas = oldRepository.AsQueryable();

                System.Console.Write("Computing difference between persona files: ");

                // Compute difference
                IEnumerableDiff<Persona, Persona> differentiator = new PersonaEnumerableDiff();
                IEnumerable<Persona> differentiatedResult = differentiator.ComputeDiff(oldPersonas, newPersonas).ToList();

                System.Console.WriteLine("{0} changes recorded.", differentiatedResult.Count());
                System.Console.WriteLine();

                // Build path to save to
                String path = (destination == Destination.Production) ? "wms/in/patron/" : "wms/test/in/patron/";
                String destinationFileName = $"itu_patrons_{DateTime.Now:\\dyyyyMMdd_\\tHHss}.xml";
                Uri uri = new Uri($@"ftp://ftp2.oclc.org/{path}{destinationFileName}");

                System.Console.WriteLine("Uploading to {0}", uri.OriginalString);

                NetworkCredential credentials = new NetworkCredential("[Username]", "[Password]");

                SFTPStreamFactory destinationFactory = new SFTPStreamFactory(uri, credentials);
                IRepository<Persona> destinationRepository = new XmlRepository<Persona, OclcPersonas>(destinationFactory, new List<Persona>());

                destinationRepository.InsertAllOnSubmit(differentiatedResult);
                destinationRepository.SubmitChanges();

                File.Delete(xmlFiles[0]);
            }

            // Output translated personas to file
            String transferFileName = $"{DateTime.Now.ToShortDateString().Replace('/', '-')}.xml";
            FileStreamFactory transferStreamFactory = new FileStreamFactory(transferFileName);
            IRepository<Persona> transferRepository = new XmlRepository<Persona, OclcPersonas>(transferStreamFactory, new List<Persona>());

            transferRepository.InsertAllOnSubmit(newPersonas);
            transferRepository.SubmitChanges();

            System.Console.WriteLine("New Persona file saved to {0}", transferFileName);
            File.Delete(lisFiles[0]);
        }
    }
}
