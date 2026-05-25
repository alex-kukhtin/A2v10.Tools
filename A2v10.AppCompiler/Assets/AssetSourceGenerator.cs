// Copyright © 2022-2026 Oleksandr Kukhtin. All rights reserved.

using System.Diagnostics;
using System.Linq;


using Microsoft.CodeAnalysis;

namespace A2v10.AppCompiler;

/*
 * https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
 * https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md* 
 */

// VS 2026 ONLY!!!!

[Generator]
public class AssetSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached)
            //Debugger.Launch();

        var files = context.AdditionalTextsProvider
            .Where(DirectoryFilter.IncludeFile)
            .Select(static (addFile, canceletionToken) =>
                (path: addFile.Path, content: addFile.GetText(canceletionToken)!.ToString()));

        // not RegisterSourceOutput!!!
        context.RegisterImplementationSourceOutput(files.Collect(), AppContainerBuilder.Build);
    }
}
