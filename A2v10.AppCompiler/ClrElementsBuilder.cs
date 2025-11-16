// Copyright © 2025 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

using Microsoft.CodeAnalysis;

namespace A2v10.AppCompiler;

internal static class ClrElementsBuilder
{
    public static void Build(SourceProductionContext context, ImmutableArray<AdditionalText> files)
    {
        if (files.Length == 0)
            return;
        var file = files[0];

        var basePath = Path.GetDirectoryName(file.Path) ?? throw new InvalidOperationException("Invalid metadata.json path");

        var metaFiles = DirectoryHelper.EnumerateFiles(Path.GetDirectoryName(file.Path), "metadata.json");
        foreach (var mf in metaFiles) 
        {
            var element = DirectoryHelper.GetFileInfo(mf, basePath);
        }
    }
}
