// Copyright © 2025-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace A2v10.AppCompiler;

#region Platform metadata

internal enum RawFieldType
{
    String
}
internal record RawField
{
    public String Name { get; init; } = String.Empty;
    public RawFieldType Type { get; init; }    
    public Int32 Length { get; init; }
}

internal record RawMetadataJson
{
    public RawField[] Fields { get; init; } = [];

}

#endregion

// FLAT! For Roslyn Equals!
internal record MetadataJson
{
    public String Directory { get; init; } = default!;
    public String Fields { get; init; } = String.Empty;
    public String? Error { get; init; }

    public Boolean IsValid => String.IsNullOrEmpty(Error);

    public RawMetadataJson ToRaw()
    {
        static RawField FromString(String ss)
        {
            var x = ss.Split('\b');
            if (x.Length != 3)
                throw new InvalidOperationException("Invalid program state");

            return new RawField()
            {
                Name = x[0],
                Type = (RawFieldType) Enum.Parse(typeof(RawFieldType), x[1], true),
                Length = Convert.ToInt32(x[2])
            };
        }
        var arr = String.IsNullOrEmpty(Fields)
            ? []
            : Fields.Split('\t').Select(FromString);

        var rm = new RawMetadataJson()
        {
            Fields = [..arr]
        };
        return rm;
    }

    internal static MetadataJson FromRaw(String dir, RawMetadataJson raw)
    {
        return new MetadataJson()
        {
            Directory = dir,
            Fields = String.Join("\t", raw.Fields.Select(f => $"{f.Name}\b{f.Type}\b{f.Length}"))
        };
    }

    internal static MetadataJson Fail(String dir, String msg)
    {
        return new MetadataJson()
        {
            Directory = dir,
            Error = msg
        };
    }

    internal static MetadataJson Parse(AdditionalText addText, CancellationToken cancellationToken)
    {
        var dir = DirectoryFilter.DirectoryName(addText.Path);

        var json = addText.GetText(cancellationToken)?.ToString();

        if (String.IsNullOrEmpty(json))
            return MetadataJson.Fail(dir, Constants.Errors.METADATA_IS_EMPTY);

        try
        {
            var rawMeta = JsonConvert.DeserializeObject<RawMetadataJson>(json!, JsonSerializerHelpers.CamelCaseSettings);
            if (rawMeta == null)
                return MetadataJson.Fail(dir, Constants.Errors.METADATA_IS_BAD);
            return MetadataJson.FromRaw(dir, rawMeta);
        }
        catch (Exception ex)
        {
            return MetadataJson.Fail(dir, ex.Message);
        }
    }
}

