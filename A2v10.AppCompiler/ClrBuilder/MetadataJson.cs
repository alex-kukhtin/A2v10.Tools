// Copyright © 2025 Oleksandr Kukhtin. All rights reserved.

using System;

namespace A2v10.AppCompiler;

public enum FieldType
{
    String
}
internal record Field
{
    public String Name { get; set; } = default!;
    public FieldType Type { get; set; }    
    public Int32 Length { get; set; }
    public Boolean Required { get; set; }
}

internal record MetadataJson
{
    public String Name { get; set; } = default!;
    public Field[] Fields { get; set; } = [];
}
