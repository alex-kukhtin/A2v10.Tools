// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A2v10.AppCompiler;

internal record ElementInfo
{
    internal ElementInfo(String fullPath, String relativePath, String identifier)
    {
        FullPath = fullPath;
        RelativePath = relativePath;
        Identifier = identifier;
    }

    internal String FullPath { get; }
    internal String RelativePath { get; }
    internal String Identifier { get; }

    public override string ToString()
    {
        return $"{RelativePath} : {Identifier}";
    }
}

internal class DirectoryHelper
{
    internal static ElementInfo GetFileInfo(String path, String basePath)
    {
        var relative = path.Replace(basePath, "").Substring(1).Replace('\\', '/');
        var ider = relative.Replace('/', '_').Replace('.', '_').Replace('-', '_');
        return new ElementInfo(path, relative, ider);
    }
    
    internal static IEnumerable<String> EnumerateFiles(String path, String fileName)
    {
        foreach (var f in Directory.EnumerateFiles(path, fileName, SearchOption.AllDirectories))
            yield return f;
    }

    static Boolean StartsWith(String path, params String[] exts)
    {
        foreach (var e in exts)
        {
            if (path.StartsWith(e, StringComparison.InvariantCultureIgnoreCase))
                return true;
        }
        return false;
    }

    internal static IEnumerable<String> EnumerateFilesMult(String path, String[] extensions)
    {
        int xLen = path.Length;
        foreach (var f in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
        {
            var relF = f.Substring(xLen + 1);
            if (StartsWith(relF, "bin", "obj"))
                continue;
            var ext = Path.GetExtension(f).ToLowerInvariant();
            if (extensions.Contains(ext))
                yield return f;
        }
    }
}

