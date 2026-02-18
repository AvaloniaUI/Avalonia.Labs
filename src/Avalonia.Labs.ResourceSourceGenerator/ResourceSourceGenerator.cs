using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Avalonia.Labs.ResourceSourceGenerator
{
    [Generator]
    public class ResourceSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValueProvider<ImmutableArray<string>> additionaFilesProvider =
            context.AdditionalTextsProvider.Select((t, _) => t.Path).Collect();

            IncrementalValueProvider<string?> rootNamespaceProvider = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
                x.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? rootNamespace)
                    ? rootNamespace
                    : null);

            IncrementalValueProvider<string?> assetLocationProvider = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
                x.GlobalOptions.TryGetValue("build_property.AvaloniaAssetLocation", out string? assetLocation)
                    ? assetLocation
                    : null);

            IncrementalValueProvider<string?> assetRootClassProvider = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
                x.GlobalOptions.TryGetValue("build_property.AvaloniaAssetRootClass", out string? assetRoot)
                    ? assetRoot
                    : null);

            IncrementalValueProvider<string?> buildProjectDirProvider = context.AnalyzerConfigOptionsProvider.Select(
            (x, _) =>
                x.GlobalOptions.TryGetValue("build_property.projectdir", out string? buildProjectDir)
                    ? buildProjectDir
                    : null);

            // We combine the providers to generate the parameters for our source generation.
            IncrementalValueProvider<(ImmutableArray<string> fileNames, string? rootNamespace, string? assetLocation, string? projectDir, string? assetRoot)>
                combined = additionaFilesProvider
                    .Combine(rootNamespaceProvider.Combine(assetLocationProvider.Combine(buildProjectDirProvider.Combine(assetRootClassProvider)))).Select((c, _) =>
                        (c.Left, c.Right.Left, c.Right.Right.Left, c.Right.Right.Right.Left, c.Right.Right.Right.Right));

            context.RegisterSourceOutput(combined, this.GenerateSourceIncremental);
        }

        private void GenerateSourceIncremental(SourceProductionContext context, (ImmutableArray<string> fileNames, string rootNamespace, string assetLocation, string projectDir, string assetRoot) tuple)
        {

            var fileNames = tuple.fileNames;
            var rootNamespace = tuple.rootNamespace;
            var assetLocation = tuple.assetLocation;
            var projectDir = tuple.projectDir;
            var assetRoot = tuple.assetRoot;

            if (!fileNames.Any() || string.IsNullOrEmpty(rootNamespace) || string.IsNullOrEmpty(assetLocation))
                return;

            var assetsFolder = Path.Combine(projectDir, assetLocation);

            var filteredFiles = fileNames.Where(x => x.StartsWith(assetsFolder)).Where(z => !string.IsNullOrEmpty(z)).OrderBy(a => a).ToList();

            if (filteredFiles.Any())
            {
                var root = new ResourceDir()
                {
                    Name = assetRoot
                };

                foreach (var file in filteredFiles)
                {
                    var relativePath = GetRelativePath(file, projectDir);
                    var assetPath = GetRelativePath(file, assetsFolder);
                    var parts = assetPath.Split([Path.DirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);

                    var node = root;

                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        node = GetNode(node, parts[i]);
                    }

                    var name = parts.LastOrDefault();

                    if (!string.IsNullOrEmpty(name))
                    {
                        node.ChildItems.Add(new ResourceItem(Sanitize(name), "/" + relativePath.Replace(Path.DirectorySeparatorChar, '/')));
                    }
                }

                ResourceDir GetNode(ResourceDir parent, string name)
                {
                    if (parent.ChildDirs.Find(x => x.Name == name) is not { } node)
                    {
                        node = new ResourceDir()
                        {
                            Name = Sanitize(name),
                        };
                        parent.ChildDirs.Add(node);
                    }

                    return node;
                }

                EnterNode(root, assetRoot);


                void EnterNode(ResourceDir node, string fileName)
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (var dir in node.ChildDirs)
                    {
                        EnterNode(dir, fileName + "." + dir.Name);
                    }

                    var isStatic = node == root;

                    stringBuilder.AppendLine($$"""
                    using System;
                    using System.Collections;
                    using System.IO;
                    using Avalonia.Labs.Controls;

                    namespace {{rootNamespace}}.{{assetRoot}};

                    public {{(isStatic ? "static " : "") }} class {{node.Name}}{{(isStatic ? "" : "Resource")}}
                    {
                    """);

                    List<string> names = new List<string>();

                    int dupeSuffix = 1;
                    foreach (var dir in node.ChildDirs)
                    {
                        stringBuilder.AppendLine($$"""
                                public {{(isStatic ? "static" : "")}} {{dir.Name}}Resource {{dir.Name}} { get; } = new {{dir.Name}}Resource();

                            """
                            );
                    }

                    foreach(var resource in node.ChildItems)
                    {
                        var name = resource.Name;
                        if (names.Contains(name))
                        {
                            name += dupeSuffix++;
                        }
                        else
                            dupeSuffix = 1;
                        names.Add(resource.Name);

                        var uri = $"avares://{rootNamespace}{resource.ResourcePath}";
                        stringBuilder.AppendLine($$"""
                                public {{(isStatic ? "static" : "")}} ResourceItem {{name}} { get; } = new ResourceItem("{{resource.Name}}", new Uri("{{uri}}"));

                            """
                            );
                    }


                    stringBuilder.AppendLine("""
                    }
                    """);

                    var sourceCode = stringBuilder.ToString();
                    var source = SourceText.From(sourceCode, Encoding.UTF8);

                    context.AddSource($"{fileName.TrimStart('.')}.g.cs",source);
                }

                string Sanitize(string text)
                {
                    var sanitizedName = Regex.Replace(Path.GetFileNameWithoutExtension(text), @"[^a-zA-Z0-9_]", "") ?? "";

                    if (!string.IsNullOrEmpty(sanitizedName) && char.IsDigit(sanitizedName[0]))
                    {
                        sanitizedName = "_" + sanitizedName;
                    }

                    return string.Concat(sanitizedName[0].ToString().ToUpperInvariant(), sanitizedName.AsSpan(1).ToString());
                }
            }
        }

        private static string GetRelativePath(string fullPath, string basePath)
        {
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            Uri baseUri = new(basePath);
            Uri fullUri = new(fullPath);

            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            return relativeUri.ToString().Replace("/", Path.DirectorySeparatorChar.ToString()).Replace('\\', Path.DirectorySeparatorChar);
        }
    }

    public class ResourceDir
    {
        public string Name { get; set; }
        public List<ResourceDir> ChildDirs { get; } = new List<ResourceDir>();
        public List<ResourceItem> ChildItems { get; } = new List<ResourceItem>();
    }

    public class ResourceItem
    {
        public ResourceItem(string name, string resourcePath)
        {
            Name = name;
            ResourcePath = resourcePath;
        }

        public string Name { get;}
        public string ResourcePath { get; }
    }
}
