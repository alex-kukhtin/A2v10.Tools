using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    public static Boolean InclideFile(AdditionalText f)
    {
        return !f.Path.Contains(BIN_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.Contains(OBJ_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.Contains(SCHEMA_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.Contains(PROPS_PART, StringComparison.OrdinalIgnoreCase) &&
            !f.Path.EndsWith(TSCONFIG_PART, StringComparison.OrdinalIgnoreCase);
    }

    public static Boolean IsMetadataFile(AdditionalText f)
    {
        return InclideFile(f) && f.Path.EndsWith(METADATAJSON_PART, StringComparison.OrdinalIgnoreCase);
    }

    public static String NormalizePath(String? a) => a?.Replace('\\', '/').ToLowerInvariant() ?? String.Empty;

    public static Boolean IsDirectoryEqual(String? a, String? b)
    {
        if (a is null || b is null)
            return false;

        String? na = NormalizePath(a);
        String? nb = NormalizePath(b);

        return String.Equals(na, nb, StringComparison.OrdinalIgnoreCase);
    }

    public static String DirectoryName(String filePath)
        => DirectoryFilter.NormalizePath(Path.GetDirectoryName(filePath));
}
