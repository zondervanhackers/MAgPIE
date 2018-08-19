#addin "Cake.SqlPackage"
#addin "Cake.Services"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Vamp");
var configuration = Argument("configuration", "Test");
var solutionFile = GetFiles("*.sln").First();


///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

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
  .Does(() =>
  {
	Information("Directories to Clean {0}", string.Join(", \n", GetDirectories(string.Format("../**/obj/{0}", configuration)).Select(x => x.Segments[6])));
	
    CleanDirectories(string.Format("**/obj/{0}",
      configuration));
    CleanDirectories(string.Format("**/bin/{0}",
      configuration));
  });

Task("Build")
	.Does(() => 
	{
		MSBuild(solutionFile, new MSBuildSettings {
			ToolVersion = MSBuildToolVersion.VS2017,
			Configuration = configuration,
			PlatformTarget = PlatformTarget.MSIL,
			MaxCpuCount = System.Environment.ProcessorCount
		});
	}
);

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
	}
);

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
	}
);

Task("Build Vamp")
	.IsDependentOn("NugetRestore")
	.IsDependentOn("Clean")
	.IsDependentOn("Build")
	.IsDependentOn("Build Website")
	.IsDependentOn("Run Unit Tests");

Task("Deploy Statistics Database")
	.Does(() => {
		var dacpacFilePath = GetFiles(string.Format("./Statistics.Database/bin/{0}/Statistics.Database.dacpac", configuration)).First();
		var sqlPackageSettings = new SqlPackagePublishSettings();
		sqlPackageSettings.ToolPath = "./DAC/SqlPackage.exe";
		sqlPackageSettings.SourceFile = dacpacFilePath;
		sqlPackageSettings.TargetConnectionString = string.Format("Data Source=LIBRARYSRV18;Initial Catalog=Statistics ({0}); User ID=TeamCityBuilder;Password=WddM8I1nhEgOVh7", configuration);

		SqlPackagePublish(sqlPackageSettings);
});

Task("Deploy Harvester Database")
	.Does(() => {
		var dacpacFilePath = GetFiles(string.Format("./Harvester.Database/bin/{0}/Harvester.Database.dacpac", configuration)).First();
		var sqlPackageSettings = new SqlPackagePublishSettings();
		sqlPackageSettings.ToolPath = "./DAC/SqlPackage.exe";
		sqlPackageSettings.SourceFile = dacpacFilePath;
		sqlPackageSettings.TargetConnectionString = string.Format("Data Source=LIBRARYSRV18;Initial Catalog=Harvester ({0}); User ID=TeamCityBuilder;Password=WddM8I1nhEgOVh7", configuration);

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


Task("Deploy Vamp")
.IsDependentOn("Deploy Statistics Database")
.IsDependentOn("Deploy Harvester Database")
.IsDependentOn("Install Harvester Service")
;

Task("Vamp")
	.IsDependentOn("Build Vamp")
	.IsDependentOn("Deploy Vamp")
	;

RunTarget(target);