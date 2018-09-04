#addin Cake.FileHelpers
#addin Cake.Npm

var target = Argument("target", "Default");
var contract = "Bravo";
var configuration = Argument("configuration", "Release");

Task("Install-Packages")
    .Does(() =>
    {
        //NpmInstall(settings => settings.AddPackage("solc").InstallGlobally());
		
		ChocolateyInstall("nodejs.install");
		
		Information("\r\nInstalling solidity compiler\r\n");
        NpmInstall("solc");
    });

Task("Clean")
	.IsDependentOn("Install-Packages")
    .Does(() =>
    {
        var settings = new DotNetCoreCleanSettings
        {
            Configuration = configuration
        };

        DotNetCoreClean("./", settings);
        CleanDirectory(Directory("bin"));
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        Information("Configuration: " + configuration + "\r\n");

        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration
        };

        DotNetCoreBuild("./", settings);
    });

Task("Contract")
    .IsDependentOn("Build")
    .Does(() => 
    {
        Information("Run compile.js\r\n");

        StartProcess("powershell", new ProcessSettings 
        {
            Arguments = new ProcessArgumentBuilder()
                .Append(@"node compile.js", contract)
        });
    });

Task("Test")
    .IsDependentOn("Contract")
    .Does(() => 
    {
		if (IsRunningOnWindows())
		{
			Information("Windows!");
		}
		
		if (IsRunningOnUnix())
		{
			Information("Unix!");
		}
	
        //var startInfo = new System.Diagnostics.ProcessStartInfo
        //{
        //    FileName = "powershell.exe",
        //    RedirectStandardInput = true,
        //    RedirectStandardOutput = true,
        //    UseShellExecute = false,
        //    CreateNoWindow = true
        //};
        //
        //var process = new System.Diagnostics.Process { StartInfo = startInfo };
        //process.Start();
        //
        //var testchainDir = MakeAbsolute(Directory("./testchain"));
        //
        //process.StandardInput.WriteLine("cd " + testchainDir);
        //
        //var startgeth = testchainDir + "/startgeth.bat";
        //Information("Running geth: " + startgeth + "\r\n");
        //
        //process.StandardInput.WriteLine(startgeth);
		//
        //var settings = new DotNetCoreTestSettings
        //{
        //    Configuration = configuration
        //};
        //
        //DotNetCoreTest("./", settings);
		//
		//// exit geth
        //process.StandardInput.WriteLine("exit");
        //// exit powershell
        //process.StandardInput.WriteLine("exit");
        //process.WaitForExit();
    });

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);