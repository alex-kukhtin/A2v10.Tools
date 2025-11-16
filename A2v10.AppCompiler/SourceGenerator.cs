// Copyright © 2022-2025 Oleksandr Kukhtin. All rights reserved.

using System.Diagnostics;

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
        if (!Debugger.IsAttached)
            Debugger.Launch();

        var items = context.AdditionalTextsProvider.Where(f => f.Path.EndsWith(".csproj")).Collect();
        context.RegisterSourceOutput(items, AppContainerBuilder.Build);
        context.RegisterSourceOutput(items, ClrElementsBuilder.Build);
    }
}
