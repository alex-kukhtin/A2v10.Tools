// Copyright © 2022-2025 Oleksandr Kukhtin. All rights reserved.

using System.Diagnostics;
using System.Linq;


using Microsoft.CodeAnalysis;

namespace A2v10.AppCompiler;

/*
 * https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
 */

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    /*
	private static readonly DiagnosticDescriptor Info = 
		new  DiagnosticDescriptor(id: "CL1001",
			title: "MESSAGE",
			messageFormat: "{0}",
			category: "A2v10.AppCompiler",
			DiagnosticSeverity.Info,
			isEnabledByDefault: true);
	*/
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached)
            //Debugger.Launch();

        var assemblyName = context.CompilationProvider.Select(static (c, _) => c.Assembly.Name!);

        var items = context.AdditionalTextsProvider.Where(static f => !f.Path.Contains("\\bin\\") && f.Path.EndsWith("metadata.json"))
            .Select((addFile, canceletionToken) => (path: addFile.Path, content: addFile.GetText(canceletionToken)));

        var module = context.AdditionalTextsProvider.Where(static f => !f.Path.Contains("\\bin\\") && f.Path.EndsWith("module.json"))
            .Select((addFile, canceletionToken) => (path: addFile.Path, content: addFile.GetText(canceletionToken)));

        var combinedClr = items.Combine(assemblyName).Collect();
        var combinedModule = module.Combine(assemblyName).Collect();

        //context.RegisterSourceOutput(combined, AppContainerBuilder.Build);
        context.RegisterSourceOutput(combinedClr, ClrElementsBuilder.Build);
    }
}
