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

    static readonly DiagnosticDescriptor NoModelJsonDiagnostic = new(
        id: "A2AC002",
        title: "No module.json found",
        messageFormat: "The file module.json not found. Check project root.",
        category: "AppCompiler",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static void Build(SourceProductionContext context, ImmutableArray<(String path, String content)> items)
    {
        if (items.Length == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(NoFilesDiagnostic, Location.None));
            return;
        }

        var (path, content) = items.FirstOrDefault(static x => x.path.EndsWith($"{Path.DirectorySeparatorChar}module.json"));

        if (String.IsNullOrEmpty(path))
        {
            context.ReportDiagnostic(Diagnostic.Create(NoModelJsonDiagnostic, Location.None));
            return; // module.json not found
        }

        var projectDir = Path.GetDirectoryName(path);

        context.AddSource("textfiles.g.cs", TextFileGenerator.GetSource(projectDir, items));

        var sb = new StringBuilder(MAIN_CODE);
        ModuleJson.ReplaceMacros(content, sb);
        context.AddSource("appcontainer.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }


    public const String MAIN_CODE = Constants.AUTO_GENERATED + """

    #nullable enable

    using System;
    using System.IO;
    using System.Collections.Generic;

    using A2v10.Module.Infrastructure;

    namespace Generated;

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

        public String GetText(String path) => _textContainer.Get(path);
        public Stream GetStream(String path) => _textContainer.GetStream(path);
        public IEnumerable<String> EnumerateFiles(String path, String searchPattern) => _textContainer.EnumerateFiles(path, searchPattern);
        public Boolean FileExists(String path) => _textContainer.Exists(path);
    }
    """;
}
