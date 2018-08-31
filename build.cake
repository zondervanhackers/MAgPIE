#addin "Cake.SqlPackage"
#addin "Cake.Services"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "MAgPIE");
var configuration = Argument("configuration", "Production");
var solutionFile = GetFiles("*.sln").First();


///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Stop Harvester Service")
	.Does(() => {
		string ServiceName = string.Format("Harvester Service ({0})", configuration);

		if (GetServices().Any(x => x.ServiceName == ServiceName))
		{
			if (CanServiceStop(ServiceName))
				StopService(ServiceName);
		}
});

Task("NugetRestore")
	.Does(() => {
		if (solutionFile is null || String.IsNullOrEmpty(solutionFile.FullPath))
		{
			Error("No Solution File Found");
		}

		Information("Restoring {0}", solutionFile);
		NuGetRestore(solutionFile);
});

Task("Clean")
	.Does(() => {
		var objDirectories = GetDirectories(string.Format("**/obj/{0}", configuration));
		var binDirectories = GetDirectories(string.Format("**/bin/{0}", configuration));

		Information("Directories to Clean {0}", string.Join(", \n", objDirectories.Select(x => { return $"{x.Segments[x.Segments.Length - 3]} ({x.Segments[x.Segments.Length - 1]})"; })));
		
		CleanDirectories(objDirectories);
		CleanDirectories(binDirectories);
});

Task("Build")
	.Does(() => {
		MSBuild(solutionFile, new MSBuildSettings {
			ToolVersion = MSBuildToolVersion.VS2017,
			Configuration = configuration,
			PlatformTarget = PlatformTarget.MSIL,
			MaxCpuCount = System.Environment.ProcessorCount,
			MSBuildPlatform = MSBuildPlatform.Automatic,
		});
});

Task("Build Website")
	.Does(() => 
	{
		MSBuild(GetFiles("Statistics.Web/Statistics.Web.csproj").First(), new MSBuildSettings()
		 	.UseToolVersion(MSBuildToolVersion.VS2017)
			.SetConfiguration(configuration)
			.SetPlatformTarget(PlatformTarget.MSIL)
			.WithTarget("Package")
			.SetMaxCpuCount(System.Environment.ProcessorCount)
			.WithProperty("PackageLocation", new[] { "Website.zip" })
		);
});

Task("Run Unit Tests")
	.Does(() =>
	{
		MSBuild(GetFiles("xUnitTeamCity.proj").First(), new MSBuildSettings()
			.UseToolVersion(MSBuildToolVersion.VS2017)
			.SetConfiguration(configuration)
			.SetPlatformTarget(PlatformTarget.MSIL)
			.WithTarget("Test")
			.SetMaxCpuCount(System.Environment.ProcessorCount)
			.WithProperty("PackageLocation", new[] { "Website.zip" })
		);
});

Task("Build MAgPIE")
	.IsDependentOn("Stop Harvester Service")
	.IsDependentOn("NugetRestore")
	.IsDependentOn("Clean")
	.IsDependentOn("Build")
	// .IsDependentOn("Build Website")
	.IsDependentOn("Run Unit Tests");

Task("Deploy Statistics Database")
	.Does(() => {
		var dacpacFilePath = GetFiles(string.Format("./Statistics.Database/bin/{0}/Statistics.Database.dacpac", configuration)).First();
		var sqlPackageSettings = new SqlPackagePublishSettings();
		sqlPackageSettings.ToolPath = "./Test/bin/Release/sqlpackage.exe";
		sqlPackageSettings.SourceFile = dacpacFilePath;
		sqlPackageSettings.TargetConnectionString = string.Format("Data Source=MAgPIEServer;Initial Catalog=Statistics ({0}); User ID=TeamCityBuilder;Password=WddM8I1nhEgOVh7", configuration);

		SqlPackagePublish(sqlPackageSettings);
});

Task("Deploy Harvester Database")
	.Does(() => {
		var dacpacFilePath = GetFiles(string.Format("./Harvester.Database/bin/{0}/Harvester.Database.dacpac", configuration)).First();
		var sqlPackageSettings = new SqlPackagePublishSettings();
		sqlPackageSettings.ToolPath = "./Test/bin/Release/sqlpackage.exe";
		sqlPackageSettings.SourceFile = dacpacFilePath;
		sqlPackageSettings.TargetConnectionString = string.Format("Data Source=MAgPIEServer;Initial Catalog=Harvester ({0}); User ID=TeamCityBuilder;Password=WddM8I1nhEgOVh7", configuration);

		SqlPackagePublish(sqlPackageSettings);
});

Task("Install Harvester Service")
	.Does(() => {
		
		string ServiceName = string.Format("Harvester Service ({0})", configuration);

		if (GetServices().Any(x => x.ServiceName == ServiceName))
		{
			if (CanServiceStop(ServiceName))
				StopService(ServiceName);
			
			UninstallService(ServiceName);
		}
		
		InstallService(new InstallSettings()
		{
			ServiceName = ServiceName,
			DisplayName = ServiceName,

			ExecutablePath = string.Format("Harvester.Service/bin/{0}/Harvester Service.exe", configuration),
			StartMode = "delayed-auto"
		});
		
		Information("Start Service: " + DateTime.Now.ToLongTimeString());
		StartService(ServiceName);
});


Task("Deploy MAgPIE")
.IsDependentOn("Deploy Statistics Database")
.IsDependentOn("Deploy Harvester Database")
.IsDependentOn("Install Harvester Service")
;

Task("MAgPIE")
	.IsDependentOn("Build MAgPIE")
	.IsDependentOn("Deploy MAgPIE")
	;

RunTarget(target);