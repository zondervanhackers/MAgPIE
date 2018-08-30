# Configuration File Structure

## Notification (SystemEmailAccount, DeveloperEmails, UserEmails)

Notification contains information about emails associated with Developers, System Managers, and users. An email will be sent to a group when something should be brought to their attention

~~~xml
<SystemEmailAccount>
      <UserName>UserName</UserName>
      <Password>Password</Password>
</SystemEmailAccount>
<DeveloperEmails>
      <Email>developer1@email.com</Email>
      <Email>developer2@email.com</Email>
</DeveloperEmails>
<UserEmails>
      <Email>user@email.com</Email>
</UserEmails>
~~~

## Repositories (SqlServerDatabase, FolderDirectory, FtpDirectory, SftpDirectory, SushiCounter, EbscoHostCounter)

Repositories contain information about where Data will be stored or retrieved. These will be used in Operation declarations. Results from Operations will be stored in Database Repositories. Counter Repositories require identifying information about a user to access a usage report service.

~~~xml
<Repository xsi:type="SqlServerDatabase">
      <Name>DatabaseName</Name>
      <Server>ServerName</Server>
      <Database>DatabaseName</Database>
      <Authentication>Windows</Authentication>
    </Repository>
<Repository xsi:type="SqlServerDatabase">
      <Name>DatabaseName</Name>
      <Server>ServerName</Server>
      <Database>DatabaseName</Database>
      <Authentication>UsernamePassword</Authentication>
      <Username>Username</Username>
      <Password>Password</Password>
</Repository>
<Repository xsi:type="FolderDirectory">
      <Name>DirectoryTitle</Name>
      <Path>C:\Directory\</Path>
</Repository>
<Repository xsi:type="FtpDirectory">
      <Name>Name</Name>
      <Host>FtpHost</Host>
      <Port>PortNumber</Port>
      <Username>Username</Username>
      <Password>Password</Password>
      <UseSsl>false or true</UseSsl>
</Repository>
<Repository xsi:type="SftpDirectory">
      <Name>Oclc</Name>
      <Host>SftpHost</Host>
      <Port>PortNumber</Port>
      <Username>Username</Username>
      <Password>Password</Password>
      <DirectoryPath>File path in sftp space to look for files. "folder/location/place"</DirectoryPath>
</Repository>
<Repository xsi:type="SushiCounter">
      <Name>VendorName</Name>
      <Url>http://</Url>
      <RequestorID>RequestorID/RequestorID>
      <CustomerID>CustomerID</CustomerID>
      <ReleaseVersion>[Insert 'R4' or 'R3']</ReleaseVersion>
      <AvailableReports>
        <CounterReport>JR1</CounterReport>
        <CounterReport>PR1</CounterReport>
      </AvailableReports>
</Repository>
<Repository xsi:type="EbscoHostCounter">
      <Name>EbscoHost</Name>
      <Username>Username</Username>
      <Password>Password</Password>
      <Sushi>
        <Url>http://</Url>
        <RequestorID>RequestorID/RequestorID>
        <CustomerID>CustomerID</CustomerID>
        <ReleaseVersion>[Insert 'R4' or 'R3']</ReleaseVersion>
        <AvailableReports>
          <CounterReport>JR1</CounterReport>
        </AvailableReports>
      </Sushi>
</Repository>
~~~

## Operations (ImportCounterTransactions, ImportDemographics, ImportWmsInventory, ImportWmsTransaction, Sync)

Operations are run on the windows service based on their schedule. Schedules that have Units that are non Divisible such as Months, Quarters, and Years can not be decimal values while Units such as seconds, Minutes, Days, Weeks can be. MaxRunsDaily defines how many times this operation can be run in a day. This is most helpful for ImportCounter operations where Vendors only allow a certain number of requests per day. Import Operations require a SourceCounter or SourceDirectory. SourceCounters and SourceDirectories define where data will be retrieved from. Statistics Database is the database in which our data will be organized and structured. This is the model for the Statistics Database [Database Model](Statistics Database). Operations that have a Harvester Database, record which files are processed so they are not rerun if they have not been modified since last operation.

~~~xml
<Operation xsi:type="ImportCounterTransactions">
      <Name>VendorImport</Name>
      <Schedules>
        <Schedule xsi:type="IntervalSchedule" Interval="31" Unit="Days" StartDate="2014-01-01T00:00:00" EndDate="2016-01-01T00:00:00" />
      </Schedules>
      <SourceCounter>Repository</SourceCounter>
      <DestinationDatabase>Repository</DestinationDatabase>
      <MaxRunsDaily>0</MaxRunsDaily>
</Operation>
<Operation xsi:type="ImportDemographics">
      <Name>DemographicImport</Name>
      <Schedules>
        <Schedule xsi:type="IntervalSchedule" Interval="7" Unit="Weeks" StartDate="2016-02-01T00:00:00" />
      </Schedules>
      <SourceDirectory>Repository</SourceDirectory>
      <DestinationDatabase>DatabaseRepository</DestinationDatabase>
      <HarvesterDatabase>DatabaseRepository</HarvesterDatabase>
</Operation>
<Operation xsi:type="ImportWmsInventory">
      <Name>WmsInventoryImport</Name>
      <Schedules>
        <Schedule xsi:type="IntervalSchedule" Interval="7" Unit="Days" StartDate="2016-02-01T00:00:00" />
      </Schedules>
      <SourceDirectory>Repository</SourceDirectory>
      <DestinationDatabase>DatabaseRepository</DestinationDatabase>
      <HarvesterDatabase>DatabaseRepository</HarvesterDatabase>
</Operation>
<Operation xsi:type="ImportWmsTransaction">
      <Name>WmsTransactionImport</Name>
      <Schedules>
        <Schedule xsi:type="IntervalSchedule" Interval="1" Unit="Days" StartDate="2016-02-01T00:00:00" />
      </Schedules>
      <SourceDirectory>Repository</SourceDirectory>
      <DestinationDatabase>DatabaseRepository</DestinationDatabase>
      <HarvesterDatabase>DatabaseRepository</HarvesterDatabase>
</Operation>
<Operation xsi:type="Sync">
      <Name>SyncDirectories</Name>
      <Schedules>
        <Schedule xsi:type="IntervalSchedule" Interval="7" Unit="Weeks" StartDate="2016-02-01T00:00:00" />
      </Schedules>
      <SourceDirectory>Repository</SourceDirectory>
      <DestinationDirectory>Repository</DestinationDirectory>
      <FilePattern>Regular Expression</FilePattern>
</Operation>
~~~