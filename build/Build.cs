using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using NukeExtensions;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main() => IsLocalBuild ?
        Execute<Build>(x => x.CopyPackagesToNuGetCache) :
        Execute<Build>(x => x.CreateNugetPackages);

    [NuGetPackage("CycloneDX", "CycloneDX.dll", Framework = "net10.0")] readonly Tool CycloneDx = null!;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;
    [Parameter]
    readonly AbsolutePath Output = RootDirectory / "artifacts" / "packages";

    // Libraries, tests and the shared Catalog sample. The platform sample heads need
    // workloads and SDKs (macOS, iOS signing, wasm-tools) that CI doesn't install.
    readonly AbsolutePath SolutionFile = RootDirectory / "Avalonia.Labs.ci.slnf";
    readonly AbsolutePath TestProject = RootDirectory / "tests" / "Avalonia.Labs.Controls.Tests" / "Avalonia.Labs.Controls.Tests.csproj";

    Target OutputParameters => _ => _
        .Executes(() =>
        {
            Log.Information("Configuration: {Configuration}", Configuration);
            Log.Information("Output: {AbsolutePath}", Output);
            Log.Information("Version: {GetVersion}", GetVersion());
        });

    Target Compile => _ => _
        .DependsOn(OutputParameters)
        .DependsOn(RunTests)
        .Executes(() => DotNetBuild(c => c
            .SetProjectFile(SolutionFile)
            .SetVersion(GetVersion())
            .SetConfiguration(Configuration)
        ));

    Target RunTests => _ => _
        .DependsOn(OutputParameters)
        .Executes(() => DotNetTest(c => c
            .SetProjectFile(TestProject)
            .SetVerbosity(DotNetVerbosity.minimal)
            .SetConfiguration(Configuration)
        ));

    Target CreateNugetPackages => _ => _
        .DependsOn(OutputParameters)
        .DependsOn(RunTests)
        .DependsOn(Compile)
        .Executes(() =>
        {
            // CreateSbom and CopyPackagesToNuGetCache glob this directory, so drop stale versions.
            Output.CreateOrCleanDirectory();
            // Packs only the projects that import PackEnable.targets.
            DotNetPack(c => c
                .SetProject(SolutionFile)
                .SetNoBuild(true)
                .SetNoRestore(true)
                .SetContinuousIntegrationBuild(true)
                .AddProperty("PackageVersion", GetVersion())
                .SetConfiguration(Configuration)
                .SetOutputDirectory(Output)
            );
        });

    // Runs whenever packages are produced (TriggeredBy), so the default CI target
    // 'CreateNugetPackages' generates and embeds SBOMs without workflow changes.
    Target CreateSbom => _ => _
        .DependsOn(CreateNugetPackages)
        .TriggeredBy(CreateNugetPackages)
        .Executes(() => SbomGenerator.Generate(
            CycloneDx,
            RootDirectory,
            Output,
            RootDirectory / "artifacts" / "sbom",
            GetVersion()));

    Target CopyPackagesToNuGetCache => _ => _
        .DependsOn(CreateNugetPackages)
        // CreateSbom embeds the SBOM into each .nupkg, so copy to the cache only afterwards.
        .After(CreateSbom)
        .Executes(() => NugetCache.InstallLibraryToNuGetCache(
            Output.GlobFiles("*.nupkg"),
            RootDirectory,
            GetVersion()));

    string GetVersion() => VersionResolver
        .GetGitHubVersion(
            baseVersionNumber: new Version(12, 0, 999),
            isPackingToLocalCache: RunningTargets.Concat(ScheduledTargets)
                .Any(t => t.Name == nameof(CopyPackagesToNuGetCache)))
        .ToString();
}
