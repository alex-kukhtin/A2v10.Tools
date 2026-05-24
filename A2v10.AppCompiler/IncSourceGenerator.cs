// Copyright © 2022-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace A2v10.AppCompiler;

/*
 * https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
 * https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md* 
 */

// VS 2026 ONLY!!!!

[Generator]
public class IncSourceGenerator : IIncrementalGenerator
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

        // 	<AdditionalFiles Include="$(ProjectPath)" />

        /*
        var items = context.AdditionalTextsProvider.Where(static f => !f.Path.Contains("\\bin\\") && f.Path.EndsWith("metadata.json"))
            .Select((addFile, canceletionToken) => (path: addFile.Path, content: addFile.GetText(canceletionToken)));
        */

        var module = context.AdditionalTextsProvider
            .Select((addFile, canceletionToken) => (path: addFile.Path, content: SourceText.From(String.Empty)));

        //var combinedClr = items.Combine(assemblyName).Collect();
        var combinedModule = module.Combine(assemblyName).Collect();

        context.RegisterSourceOutput(combinedModule, AppContainerBuilder.Build);
        //context.RegisterSourceOutput(combinedClr, ClrElementsBuilder.Build);
    }
}
