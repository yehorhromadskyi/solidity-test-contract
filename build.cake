#addin Cake.FileHelpers

var target = Argument("target", "Default");
var contract = "Sqrt";
var configuration = Argument("configuration", "Release");

Task("Clean")
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

Task("Compile-Contract")
    .IsDependentOn("Build")
    .Does(() => 
    {
        StartProcess("powershell", new ProcessSettings 
        {
            Arguments = new ProcessArgumentBuilder()
                .Append(@"solcjs --bin contracts/{0}.sol -o bin;", contract)
                .Append(@"solcjs --abi contracts/{0}.sol -o bin;", contract)
        });
    });

Task("Run-Geth-And-Test")
    .IsDependentOn("Compile-Contract")
    .Does(() => 
    {
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new System.Diagnostics.Process { StartInfo = startInfo };

        process.Start();
        process.StandardInput.WriteLine("cd testchain");
        process.StandardInput.WriteLine("startgeth");
        
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration
        };

        DotNetCoreTest("./", settings);

        // exit geth
        process.StandardInput.WriteLine("exit");
        // exit cmd
        process.StandardInput.WriteLine("exit");
        process.WaitForExit();
    });

Task("Default")
    .IsDependentOn("Run-Geth-And-Test");

RunTarget(target);