// Copyright © 2025-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace A2v10.AppCompiler;

internal static class AppContainerBuilder
{
    static readonly DiagnosticDescriptor NoFilesDiagnostic = new(
        id: "A2AC001",
        title: "No files to compile",
        messageFormat: "No files were found to compile. Check <AdditionalFiles> in the project file.",
        category: "AppCompiler",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    static readonly DiagnosticDescriptor BadModuleJsonDiagnostic = new(
        id: "A2AC002",
        title: "module.json missing or invalid",
        messageFormat: "module.json {0}. AppContainer will not be generated.",
        category: "AppCompiler",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static void Build(SourceProductionContext context, ImmutableArray<(String path, String content)> items, String ns)
    {
        if (items.Length == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(NoFilesDiagnostic, Location.None));
            return;
        }

        var (path, content) = items.FirstOrDefault(static x => x.path.EndsWith($"{Path.DirectorySeparatorChar}module.json"));

        if (String.IsNullOrEmpty(path))
        {
            context.ReportDiagnostic(Diagnostic.Create(BadModuleJsonDiagnostic, Location.None, "not found in project root"));
            return;
        }

        var md = ModuleJson.Load(content);
        if (md == null)
        {
            context.ReportDiagnostic(Diagnostic.Create(BadModuleJsonDiagnostic, Location.None, "is invalid or has unsupported format"));
            return;
        }

        var projectDir = Path.GetDirectoryName(path);

        context.AddSource("A2v10.TextFiles.g.cs", TextFileGenerator.GetSource(projectDir, items, ns));

        var sb = new StringBuilder(MAIN_CODE.Replace("$namespace$", ns));
        md.ReplaceMacros(sb);

        context.AddSource("A2v10.AppContainer.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }


    public const String MAIN_CODE = Constants.AUTO_GENERATED + """

    #nullable enable

    using System;
    using System.IO;
    using System.Collections.Generic;

    using A2v10.Module.Infrastructure;

    namespace $namespace$;

    public class AppContainer : IAppContainer 
    {
        public Boolean IsLicensed => true;
        public Guid Id => _id;	
        public String ModuleName => "$(name)";
        public String? Authors => "$(authors)";
        public String? Company => "$(company)";
        public String? Description => "$(description)";
        public String? Copyright => "$(copyright)";

        private static readonly Guid _id = new Guid("$(id)");
        private readonly TextFilesContainer _textContainer = new();

        public String? GetText(String path) => _textContainer.Get(path);
        public Stream? GetStream(String path) => _textContainer.GetStream(path);
        public IEnumerable<String> EnumerateFiles(String path, String searchPattern) => _textContainer.EnumerateFiles(path, searchPattern);
        public Boolean FileExists(String path) => _textContainer.Exists(path);
    }
    """;
}
