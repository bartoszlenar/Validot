using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    static readonly Regex SemVerRegex = new Regex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$", RegexOptions.Compiled);

    static readonly Regex TargetFrameworkRegex = new Regex(@"<TargetFramework>.+<\/TargetFramework>", RegexOptions.Compiled);

    static readonly DateTimeOffset BuildTime = DateTimeOffset.UtcNow;

    static readonly string DefaultFrameworkId = "net5.0";

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build. 'Debug' (default) or 'Release'.")]
    readonly Configuration Configuration = Configuration.Debug;

    [Parameter("dotnet framework id or SDK version (if SDK version is provided, the highest framework available is selected). Default value is 'netcoreapp3.1'")]
    string DotNet;

    [Parameter("Version. Default value is '0.0.0-timestamp'")]
    string Version;

    [Parameter("NuGet API. Where to publish NuGet package. Default value is 'https://api.nuget.org/v3/index.json'")]
    readonly string NuGetApi = "https://api.nuget.org/v3/index.json";

    [Parameter("NuGet API key, allows to publish NuGet package.")]
    readonly string NuGetApiKey;

    [Parameter("CodeCov API key, allows to publish code coverage.")]
    readonly string CodeCovApiKey;

    [Parameter("Commit SHA")]
    readonly string CommitSha;

    [Parameter("If true, BenchmarkDotNet will run full (time consuming, but more accurate) jobs.")]
    readonly bool FullBenchmark;

    [Parameter("Benchmark filter. If empty, all benchmarks will be run.")]
    readonly string BenchmarksFilter;

    [Parameter("Allow warnings")]
    readonly bool AllowWarnings;

    [Solution]
    readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ToolsPath => RootDirectory / "tools";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestsResultsDirectory => ArtifactsDirectory / "tests";
    AbsolutePath CodeCoverageDirectory => ArtifactsDirectory / "coverage";
    AbsolutePath CodeCoverageReportsDirectory => ArtifactsDirectory / "coverage_reports";
    AbsolutePath BenchmarksDirectory => ArtifactsDirectory / "benchmarks";
    AbsolutePath NuGetDirectory => ArtifactsDirectory / "nuget";

    protected override void OnBuildInitialized()
    {
        base.OnBuildCreated();

        DotNet = GetFramework(DotNet);
        Logger.Info($"DotNet: {DotNet}");

        Version = GetVersion(Version);
        Logger.Info($"Version: {Version}");

        Logger.Info($"NuGetApi: {NuGetApi ?? "MISSING"}");
        Logger.Info($"Configuration: {Configuration}");
        Logger.Info($"CommitSha: {CommitSha ?? "MISSING"}");
        Logger.Info($"AllowWarnings: {AllowWarnings}");

        Logger.Info($"FullBenchmark: {FullBenchmark}");
        Logger.Info($"BenchmarkFilter: {FullBenchmark}");

        var nuGetApiKeyPresence = (NuGetApiKey is null) ? "MISSING" : "present";
        Logger.Info($"NuGetApiKey: {nuGetApiKeyPresence}");

        var codeCovApiKeyPresence = (CodeCovApiKey is null) ? "MISSING" : "present";
        Logger.Info($"CodeCovApiKey: {codeCovApiKeyPresence}");

        SetFrameworkInTests(DotNet);
        SetVersionInAssemblyInfo(Version, CommitSha);
    }

    protected override void OnBuildFinished()
    {
        ResetFrameworkInTests();
        ResetVersionInAssemblyInfo();

        base.OnBuildFinished();
    }

    Target Reset => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(TemporaryDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(ToolsPath);
            ResetFrameworkInTests();
        })
        .Triggers(Clean);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target CompileProject => _ => _
        .DependsOn(Clean, Restore)
        .Executes(() =>
        {
            DotNetBuild(c => c
                .EnableNoRestore()
                .SetTreatWarningsAsErrors(!AllowWarnings)
                .SetProjectFile(SourceDirectory / "Validot/Validot.csproj")
                .SetConfiguration(Configuration)
                .SetFramework("netstandard2.0")
            );
        });

    Target CompileTests => _ => _
        .DependsOn(Clean, Restore)
        .After(CompileProject)
        .Executes(() =>
        {
            var testsProjects = new[]
            {
                TestsDirectory / "Validot.Tests.Unit/Validot.Tests.Unit.csproj",
                TestsDirectory / "Validot.Tests.Functional/Validot.Tests.Functional.csproj"
            };

            foreach (var testProject in testsProjects)
            {
                DotNetBuild(c => c
                    .EnableNoRestore()
                    .SetTreatWarningsAsErrors(!AllowWarnings)
                    .SetProjectFile(testProject)
                    .SetConfiguration(Configuration)
                    .SetFramework(DotNet)
                    .AddProperty("DisableSourceLink", "1")
                );
            }
        });

    Target Compile => _ => _
        .DependsOn(CompileProject, CompileTests);

    Target Tests => _ => _
        .DependsOn(CompileTests)
        .Executes(() =>
        {
            DotNetTest(p => p
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetProjectFile(TestsDirectory / "Validot.Tests.Unit/Validot.Tests.Unit.csproj")
                .SetFramework(DotNet)
                .SetLogger($"junit;LogFilePath={TestsResultsDirectory / $"Validot.{Version}.testresults"/ $"Validot.{Version}.unit.junit"}")
            );

            DotNetTest(p => p
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetProjectFile(TestsDirectory / "Validot.Tests.Functional/Validot.Tests.Functional.csproj")
                .SetFramework(DotNet)
                .SetLogger($"junit;LogFilePath={TestsResultsDirectory / $"Validot.{Version}.testresults" / $"Validot.{Version}.functional.junit"}")
            );
        });

    Target CodeCoverage => _ => _
        .DependsOn(CompileTests)
        .Requires(() => Configuration == Configuration.Debug)
        .Executes(() =>
        {
            var reportFile = CodeCoverageDirectory / $"Validot.{Version}.opencover.xml";

            DotNetTest(p => p
                .EnableNoBuild()
                .SetProjectFile(TestsDirectory / "Validot.Tests.Unit/Validot.Tests.Unit.csproj")
                .SetConfiguration(Configuration.Debug)
                .SetFramework(DotNet)
                .AddProperty("CollectCoverage", "true")
                .AddProperty("CoverletOutput", reportFile)
                .AddProperty("CoverletOutputFormat", "opencover")
                .AddProperty("DisableSourceLink", "1")
            );

            Logger.Info($"CodeCoverage opencover format file location: {reportFile} ({new FileInfo(reportFile).Length} bytes)");
        });

    Target CodeCoverageReport => _ => _
        .DependsOn(CodeCoverage)
        .Requires(() => Configuration == Configuration.Debug)
        .Executes(() =>
        {
            var toolPath = InstallAndGetToolPath("dotnet-reportgenerator-globaltool", "4.8.1", "ReportGenerator.dll", "net5.0");

            var toolParameters = new[]
            {
                $"-reports:{CodeCoverageDirectory / $"Validot.{Version}.opencover.xml"}",
                $"-reporttypes:HtmlInline_AzurePipelines;JsonSummary",
                $"-targetdir:{CodeCoverageReportsDirectory / $"Validot.{Version}.coverage_report"}",
                $"-historydir:{CodeCoverageReportsDirectory / "_history"}",
                $"-title:Validot unit tests code coverage report",
                $"-tag:v{Version}" + (CommitSha is null ? "" : $"+{CommitSha}"),
            };

            ExecuteTool(toolPath, string.Join(" ", toolParameters.Select(p => $"\"{p}\"")));

            File.Move(CodeCoverageReportsDirectory / $"Validot.{Version}.coverage_report/Summary.json", CodeCoverageReportsDirectory / $"Validot.{Version}.coverage_summary.json");
        });

    Target Benchmarks => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            var benchmarksPath = BenchmarksDirectory / $"Validot.{Version}.benchmarks";

            var jobShort = FullBenchmark ? string.Empty : "--job short";
            var filter = BenchmarksFilter is null ? "*" : BenchmarksFilter;

            DotNetRun(p => p
                .SetProjectFile(TestsDirectory / "Validot.Benchmarks/Validot.Benchmarks.csproj")
                .SetConfiguration(Configuration.Release)
                .SetProcessArgumentConfigurator(a => a
                    .Add("--")
                    .Add($"--artifacts {benchmarksPath} {jobShort}")
                    .Add("--exporters GitHub StackOverflow JSON HTML")
                    .Add($"--filter {filter}")
                )
            );
        });

    Target NugetPackage => _ => _
        .DependsOn(Compile)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            DotNetPack(p => p
                .EnableNoBuild()
                .SetConfiguration(Configuration.Release)
                .SetProject(SourceDirectory / "Validot/Validot.csproj")
                .SetVersion(Version)
                .SetOutputDirectory(NuGetDirectory / Version)
            );
        });

    Target PublishNugetPackage => _ => _
        .DependsOn(NugetPackage)
        .Requires(() => NuGetApiKey != null)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            DotNetNuGetPush(p => p
                .SetSource(NuGetApi)
                .SetApiKey(NuGetApiKey)
                .SetTargetPath(NuGetDirectory / Version / $"Validot.{Version}.nupkg")
            );
        });

    Target PublishCodeCoverage => _ => _
        .DependsOn(CodeCoverage)
        .Requires(() => CodeCovApiKey != null)
        .Requires(() => Configuration == Configuration.Debug)
        .Executes(() =>
        {
            var reportFile = CodeCoverageDirectory / $"Validot.{Version}.opencover.xml";

            var toolPath = InstallAndGetToolPath("codecov.tool", "1.12.3", "codecov.dll", "netcoreapp3.0");

            var toolParameters = new[]
            {
                $"--sha {CommitSha}",
                $"--file {reportFile}",
                $"--token {CodeCovApiKey}",
                $"--required"
            };

            ExecuteTool(toolPath, string.Join(" ", toolParameters));
        });

    void SetFrameworkInTests(string framework)
    {
        var testsCsprojs = new[]
        {
            TestsDirectory / "Validot.Tests.Unit/Validot.Tests.Unit.csproj",
            TestsDirectory / "Validot.Tests.Functional/Validot.Tests.Functional.csproj",
            TestsDirectory / "Validot.Benchmarks/Validot.Benchmarks.csproj",
        };

        foreach (var csproj in testsCsprojs)
        {
            SetFrameworkInCsProj(framework, csproj);
        }
    }

    void SetFrameworkInCsProj(string framework, string csProjPath)
    {
        Logger.Info($"Setting framework {framework} in {csProjPath}");

        var content = TargetFrameworkRegex.Replace(File.ReadAllText(csProjPath), $"<TargetFramework>{framework}</TargetFramework>");

        File.WriteAllText(csProjPath, content);
    }

    void SetVersionInAssemblyInfo(string version, string commitSha)
    {
        var assemblyVersion = "0.0.0.0";
        var assemblyFileVersion = "0.0.0.0";

        if (SemVerRegex.IsMatch(version))
        {
            assemblyVersion = version.Substring(0, version.IndexOf(".", StringComparison.InvariantCulture)) + ".0.0.0";

            assemblyFileVersion = version.Contains("-", StringComparison.InvariantCulture)
                ? version.Substring(0, version.IndexOf("-", StringComparison.InvariantCulture)) + ".0"
                : version + ".0";
        }

        Logger.Info("Setting AssemblyVersion: " + assemblyVersion);
        Logger.Info("Setting AssemblyFileVersion: " + assemblyFileVersion);

        var assemblyInfoPath = SourceDirectory / "Validot/Properties/AssemblyInfo.cs";

        var assemblyInfoLines = File.ReadAllLines(assemblyInfoPath);

        var autogeneratedPostfix = "// this line is autogenerated by the build script";

        for (var i = 0; i < assemblyInfoLines.Length; ++i)
        {
            if (assemblyInfoLines[i].Contains("AssemblyVersion", StringComparison.InvariantCulture))
            {
                assemblyInfoLines[i] = $"[assembly: System.Reflection.AssemblyVersion(\"{assemblyVersion}\")] {autogeneratedPostfix}";
            }
            else if (assemblyInfoLines[i].Contains("AssemblyFileVersion", StringComparison.InvariantCulture))
            {
                assemblyInfoLines[i] = $"[assembly: System.Reflection.AssemblyFileVersion(\"{assemblyFileVersion}\")] {autogeneratedPostfix}";
            }
        }

        File.WriteAllLines(assemblyInfoPath, assemblyInfoLines);
    }

    void ResetVersionInAssemblyInfo() => SetVersionInAssemblyInfo("0.0.0", null);

    void ResetFrameworkInTests() => SetFrameworkInTests("net5.0");

    string GetFramework(string dotnet)
    {
        if (string.IsNullOrWhiteSpace(dotnet))
        {
            Logger.Warn("DotNet: parameter not provided");
            return DefaultFrameworkId;
        }

        if (char.IsDigit(dotnet.First()))
        {
            Logger.Info($"DotNet parameter recognized as SDK version: " + dotnet);

            if (dotnet.StartsWith("2.1.", StringComparison.Ordinal))
            {
                return "netcoreapp2.1";
            }

            if (dotnet.StartsWith("3.1.", StringComparison.Ordinal))
            {
                return "netcoreapp3.1";
            }

            if (dotnet.StartsWith("5.0.", StringComparison.Ordinal))
            {
                return "net5.0";
            }

            Logger.Warn("Unrecognized dotnet SDK version: " + dotnet);

            return dotnet;
        }

        if (dotnet.StartsWith("netcoreapp", StringComparison.Ordinal) && dotnet["netcoreapp".Length..].All(c => char.IsDigit(c) || c == '.'))
        {
            Logger.Info("DotNet parameter recognized as .NET Core target: " + DotNet);

            return dotnet;
        }

        if (dotnet.StartsWith("net", StringComparison.Ordinal) && DotNet["net".Length..].All(char.IsDigit))
        {
            Logger.Info("DotNet parameter recognized as .NET Framework target: " + dotnet);

            return dotnet;
        }

        Logger.Warn("Unrecognized dotnet framework id: " + dotnet);

        return dotnet;
    }

    string GetVersion(string version)
    {
        if (version is null)
        {
            Logger.Warn("Version: not provided.");

            return $"0.0.0-{BuildTime.DayOfYear}{BuildTime.ToString("HHmmss", CultureInfo.InvariantCulture)}";
        }

        return version;
    }

    void ExecuteTool(string toolPath, string parameters)
    {
        ProcessTasks.StartProcess(ToolPathResolver.GetPathExecutable("dotnet"), toolPath + " -- " + parameters).AssertZeroExitCode();
    }

    string InstallAndGetToolPath(string name, string version,  string executableFileName, string framework = null)
    {
        var frameworkPart = framework is null ? $" (framework {framework})" : string.Empty;

        var toolStamp = $"{name} {version}{frameworkPart}, executable file: {executableFileName}";

        Logger.Info($"Looking for tool: {toolStamp}");

        var toolPath = GetToolPath();

        if (toolPath is null)
        {
            DotNetToolInstall(c => c
                    .SetPackageName(name)
                    .SetVersion(version)
                    .SetToolInstallationPath(ToolsPath)
                    .SetGlobal(false));
        }

        toolPath = GetToolPath();

        if (toolPath is null)
        {
            Logger.Error($"Unable to find tool path: {name} {version} {executableFileName} {framework}");
        }

        return toolPath;

        string GetToolPath()
        {
            var frameworkPart = framework != null ? (framework + "/**/") : string.Empty;

            return GlobFiles(ToolsPath, $"**/{name}/{version}/**/{frameworkPart}{executableFileName}").FirstOrDefault();
        }
    }
}
