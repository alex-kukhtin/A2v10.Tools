
using System;
using System.IO;
using Microsoft.CodeAnalysis;

namespace A2v10.AppCompiler;

internal static class DirectoryFilter
{
    static readonly String BIN_PART = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}";
    static readonly String OBJ_PART = $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}";
    static readonly String SCHEMA_PART = $"{Path.DirectorySeparatorChar}@schemas{Path.DirectorySeparatorChar}";
    static readonly String PROPS_PART = $"{Path.DirectorySeparatorChar}Properties{Path.DirectorySeparatorChar}";
    static readonly String TSCONFIG_PART = $"{Path.DirectorySeparatorChar}tsconfig.json";
    static readonly String METADATAJSON_PART = $"{Path.DirectorySeparatorChar}metadata.json";

    internal static Boolean IncludeFile(AdditionalText f)
    {
        return !f.Path.Contains(BIN_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.Contains(OBJ_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.Contains(SCHEMA_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.Contains(PROPS_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.EndsWith(TSCONFIG_PART, StringComparison.OrdinalIgnoreCase);
    }

    internal static Boolean IsMetadataFile(AdditionalText f)
    {
        return IncludeFile(f) && f.Path.EndsWith(METADATAJSON_PART, StringComparison.OrdinalIgnoreCase);
    }

    internal static String NormalizePath(String? a) => a?.Replace('\\', '/').ToLowerInvariant() ?? String.Empty;

    internal static Boolean IsDirectoryEqual(String? a, String? b)
    {
        if (a is null || b is null)
            return false;

        String? na = NormalizePath(a);
        String? nb = NormalizePath(b);

        return String.Equals(na, nb, StringComparison.OrdinalIgnoreCase);
    }

    public static String DirectoryName(String filePath)
        => DirectoryFilter.NormalizePath(Path.GetDirectoryName(filePath));

    public static String RelativeDirectory(String dir, String ns)
    {
        var ix = dir.IndexOf(ns, 0, StringComparison.OrdinalIgnoreCase);
        if (ix == -1) 
            return dir;
        return dir.Substring(ix + ns.Length + 1);
    }

    public static String RelativeDirectorySegment(String dir, String ns)
    {
        var rel = RelativeDirectory(dir, ns);
        if (String.IsNullOrEmpty(rel))
            return "Undefined";
        var spl = rel.Split('/');
        if (spl.Length > 0)
            return spl[0];
        return "Undefined";
    }
}
