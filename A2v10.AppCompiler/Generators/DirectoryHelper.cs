// Copyright © 2022-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Linq;

namespace A2v10.AppCompiler;

internal enum ElementSchema
{
    Unknown,
    Catalog,
    Document,
    Journal
}

internal record AssetInfo(String RelativePath, String Identifier);

internal record ElementInfo
{
    internal ElementInfo(String fullPath, String relativePath, String identifier)
    {
        FullPath = fullPath;
        RelativePath = relativePath;
        Identifier = identifier;
        var elems = relativePath.ToLowerInvariant().Split('/', '\\');
        Schema = elems[0] switch
        {
            "catalog" => ElementSchema.Catalog,
            "document" => ElementSchema.Document,
            _ => ElementSchema.Unknown
        };

        CSharpClassName = String.Join("_", elems.Take(elems.Length - 1));
    }

    internal String FullPath { get; }
    internal String RelativePath { get; }
    internal String Identifier { get; }
    internal ElementSchema Schema { get; }
    internal String CSharpClassName { get; }

    public override string ToString()
    {
        return $"{RelativePath} : {Identifier}";
    }
}


