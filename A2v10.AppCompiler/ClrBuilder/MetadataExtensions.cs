// Copyright © 2025 Oleksandr Kukhtin. All rights reserved.

using System;

namespace A2v10.AppCompiler;

internal static class MetadataExtensions
{
    public static String ToPropertyType(this FieldType ft) => ft switch
    {
        FieldType.String => "String?",
        _ => "object"
    };

    public static String ToSchemaName(this String name) => name switch
    {
        "catalog" => "Catalog",
        _ => throw new InvalidOperationException("Unknown schema name")
    };
}
