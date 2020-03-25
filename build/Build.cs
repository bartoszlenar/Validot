using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Tooling;

using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    static class Metadata 
    {
        public static string Title => "Validot";

        public static string Description => "Tiny lib for great validations";

        public static string Author => "Bartosz Lenar";

        public static string RepositoryUrl => "http://github.com/bartoszlenar/Validot";

        public static string PackageIconUrl => "https://github.com/bartoszlenar/Validot/raw/master/logo/icon.png";

        public static string PackageLicenceUrl => "https://github.com/bartoszlenar/Validot/blob/master/LICENSE";

        public static string[] Tags => new [] { "validot", "validation", "validator", "fluent", "fluent-api" };
    }

    static readonly Regex SemVerRegex = new Regex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$", RegexOptions.Compiled);

    static readonly DateTimeOffset BuildTime = DateTimeOffset.UtcNow;
    
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter]
    readonly Configuration Configuration = Configuration.Debug;
    
    [Parameter("dotnet framework id or SDK version (if SDK version is provided, the highest framework available is selected). Default value is 'netcoreapp3.1'")]
    readonly string DotNet;
    
    [Parameter("Version. Default value is '0.0.0-timestamp'")]
    readonly string Version;

    [Parameter("NuGet API. Where to publish NuGet package. Default value is 'https://api.nuget.org/v3/index.json'")]
    readonly string NuGetApi;

    [Parameter("NuGet API key, allows to publish NuGet package.")]
    readonly string NuGetApiKey;

    [Parameter("CodeCov API key, allows to publish code coverage.")]
    readonly string CodeCovApiKey;
    
    [Parameter("Commit SHA")]
    readonly string CommitSha;
    
    [Parameter("Allow warnings")]
    readonly bool AllowWarnings = false;

    [Solution] readonly Solution Solution;
    
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ToolsPath => RootDirectory / "tools";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestsResultsDirectory => ArtifactsDirectory / "tests";
    AbsolutePath CodeCoverageDirectory => ArtifactsDirectory / "coverage";
    AbsolutePath CodeCoverageReportsDirectory => ArtifactsDirectory / "coverage_reports";
    AbsolutePath NuGetDirectory => ArtifactsDirectory / "nuget";

    
    Target Reset => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(TemporaryDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(ToolsPath);
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
        .Unlisted()
        .DependsOn(Clean, Restore)
        .Executes(() =>
        {
            var framework = GetFramework();
            var version = GetVersion();

            var assemblyVersion = SemVerRegex.IsMatch(version) 
                ? version.Substring(0, version.IndexOf(".", StringComparison.InvariantCulture)) + ".0.0.0"
                : "0.0.0.0";

            Logger.Info("Assembly version: " + assemblyVersion);

            DotNetBuild(c => c
                .EnableNoRestore()
                .SetTreatWarningsAsErrors(!AllowWarnings)
                .SetProjectFile(SourceDirectory / "Validot/Validot.csproj")
                .SetConfiguration(Configuration)
                .SetFramework("netstandard2.0")
                .SetPackageId(Metadata.Title)
                .SetTitle(Metadata.Title)
                .SetDescription(Metadata.Description)
                .SetRepositoryUrl(Metadata.RepositoryUrl)
                .SetPackageIconUrl(Metadata.PackageIconUrl)
                .SetAuthors(Metadata.Author)
                .SetInformationalVersion(version)
                .SetAssemblyVersion(assemblyVersion));
        });
    
    Target CompileTests => _ => _
        .Unlisted()
        .DependsOn(Clean, Restore)
        .After(CompileProject)
        .Executes(() =>
        {
            var framework = GetFramework();

            var testsProjects = new[]
            {
                TestsDirectory / "Validot.Tests.Unit/Validot.Tests.Unit.csproj"
            };

            foreach (var testProject in testsProjects)
            {
                DotNetBuild(c => c
                    .EnableNoRestore()
                    .SetTreatWarningsAsErrors(!AllowWarnings)
                    .SetProjectFile(testProject)
                    .SetConfiguration(Configuration)
                    .SetFramework(framework));
            }
        });

    Target Compile => _ => _
        .DependsOn(CompileProject, CompileTests);
    
    Target Tests => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            var framework = GetFramework();
            var version = GetVersion();

            DotNetTest(p => p
                .EnableNoBuild()
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .SetFramework(framework)
                .SetLogger($"trx;LogFileName={TestsResultsDirectory / $"Validot.{version}.tests.trx"}")
            );
        });

     Target CodeCoverage => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => Configuration == Configuration.Debug)
        .Executes(() =>
        {
            var framework = GetFramework();
            var version = GetVersion();

            var reportFile = CodeCoverageDirectory / $"Validot.{version}.opencover.xml";

            DotNetTest(p => p
                .EnableNoBuild()
                .SetProjectFile(TestsDirectory / "Validot.Tests.Unit/Validot.Tests.Unit.csproj")
                .SetConfiguration(Configuration.Debug)
                .SetFramework(framework)
                .AddProperty("CollectCoverage", "true")
                .AddProperty("CoverletOutput", reportFile)
                .AddProperty("CoverletOutputFormat", "opencover")
            );

            File.Move(CodeCoverageDirectory / $"Validot.{version}.opencover.{framework}.xml", reportFile);

            Logger.Info("CodeCoverage opencover format file location: " + reportFile);
        });

    Target CodeCoverageReport => _ => _
        .DependsOn(CodeCoverage)
        .OnlyWhenDynamic(() => Configuration == Configuration.Debug)
        .Executes(() =>
        {
            var framework = GetFramework();
            var version = GetVersion();

            var reportFile = CodeCoverageDirectory / $"Validot.{version}.opencover.xml";

            var toolPath = InstallAndGetToolPath("dotnet-reportgenerator-globaltool", "4.5.1", "ReportGenerator.dll", "netcoreapp3.0");

            var toolParameters = new[] 
            {
                $"-reports:{CodeCoverageDirectory / $"Validot.{version}.opencover.xml"}",
                $"-reporttypes:HtmlInline_AzurePipelines;JsonSummary",
                $"-targetdir:{CodeCoverageReportsDirectory / $"Validot.{version}.coverage_report"}",
                $"-historydir:{CodeCoverageReportsDirectory / "_history"}",
                $"-title:Validot unit tests code coverage report",
                $"-tag:v{version}" + (CommitSha is null ? "" : $", {CommitSha}"),
            };

            ExecuteTool(toolPath, string.Join(" ", toolParameters.Select(p => $"\"{p}\"")));

            File.Move(CodeCoverageReportsDirectory / $"Validot.{version}.coverage_report/Summary.json", CodeCoverageReportsDirectory / $"Validot.{version}.coverage_summary.json");
        });

    Target Package => _ => _
        .DependsOn(Compile)
        .OnlyWhenDynamic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            var version = GetVersion();

            DotNetPack(p => p
                .EnableNoBuild()
                .EnableIncludeSymbols()
                .SetConfiguration(Configuration.Release)
                .SetProject(SourceDirectory / "Validot/Validot.csproj")
                .SetVersion(version)
                .SetOutputDirectory(NuGetDirectory / version)
                .SetTitle(Metadata.Title)
                .SetDescription(Metadata.Description)
                .SetRepositoryUrl(Metadata.RepositoryUrl)
                .SetPackageIconUrl(Metadata.PackageIconUrl)
                .SetAuthors(Metadata.Author)
                .SetPackageLicenseUrl(Metadata.PackageLicenceUrl)
                .SetPackageTags(Metadata.Tags)
            );
        });

    Target PublishPackage => _ => _
        .DependsOn(Package)
        .OnlyWhenDynamic(() => NuGetApiKey != null)
        .OnlyWhenDynamic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            var version = GetVersion();

            DotNetNuGetPush(p => p
                .SetSource(NuGetApi)
                .SetApiKey(NuGetApiKey)
                .SetTargetPath(NuGetDirectory / version / $"Validot.{version}.nupkg")
            );
        });

    Target PublishCodeCoverage => _ => _
        .DependsOn(CodeCoverage)
        .OnlyWhenDynamic(() => CodeCovApiKey != null)
        .OnlyWhenDynamic(() => Configuration == Configuration.Debug)
        .Executes(() =>
        {
            var framework = GetFramework();
            var version = GetVersion();

            var reportFile = CodeCoverageDirectory / $"Validot.{version}.opencover.xml";

            var toolPath = InstallAndGetToolPath("codecov.tool", "1.10.0", "codecov.dll", "netcoreapp3.0");

            var toolParameters = new[] 
            {
                $"--sha {CommitSha}",
                $"--file {reportFile}",
                $"--token {CodeCovApiKey}",
                $"--required"
            };

            ExecuteTool(toolPath, string.Join(" ", toolParameters));
        });
    
    string _framework = null;

    string GetFramework()
    {
        if (_framework is null)
        {
            var defaultFramework = "netcoreapp3.1";

            if (DotNet is null)
            {
                Logger.Warn("DotNet: parameter not provided");
                _framework = defaultFramework;
            }
            else if (DotNet.All(c => char.IsDigit(c) || c == '.'))
            {
                Logger.Info($"DotNet parameter recognized as SDK version: " + DotNet);

                if (DotNet.StartsWith("2.1."))
                {
                    _framework = "netcoreapp2.1";
                }
                else if (DotNet.StartsWith("3.1."))
                {
                    _framework = "netcoreapp3.1";
                }
                else
                {
                    Logger.Warn("Unrecognized dotnet SDK version: " + DotNet);
                    
                    _framework = DotNet;
                }
            }
            else
            {
                Logger.Warn("Unrecognized dotnet framework id: " + DotNet);

                _framework = DotNet;
            }

            Logger.Info("DotNet: " + _framework);
        }

        return _framework;
    }
    
    string _version = null;

    string GetVersion()
    {
        if (_version is null)
        {
            if (Version is null)
            {
                Logger.Warn("Version: not provided.");
                _version = $"0.0.0-{BuildTime.DayOfYear}{BuildTime.ToString("HHmmss", CultureInfo.InvariantCulture)}";
            }
            else
            {
                _version = Version;
            }

            Logger.Info("Version: " + _version);
        }

        return _version;
    }

    string _nuGetApi = null;

    string GetNuGetApi() 
    {
        if (_nuGetApi is null) 
        {
            if (NuGetApi is null) 
            {
                Logger.Warn("NuGetServer: not provided.");

                _nuGetApi = "https://api.nuget.org/v3/index.json";
            }
            else 
            {
                _nuGetApi = NuGetApi;
            }

            Logger.Info("NuGetApi:" + _nuGetApi);
        }

        return _nuGetApi;
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
