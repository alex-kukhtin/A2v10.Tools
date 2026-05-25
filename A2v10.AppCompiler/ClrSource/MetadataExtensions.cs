// Copyright © 2025 Oleksandr Kukhtin. All rights reserved.

using System;

namespace A2v10.AppCompiler;

internal static class MetadataExtensions
{
    internal static String ToPropertyType(this RawFieldType ft) => ft switch
    {
        RawFieldType.String => "String?",
        _ => $"Object /*{ft}*/"
    };
}
