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
    public String Fields { get; private set; } = String.Empty;
    public String? Error { get; private set; }

    public Boolean IsValid => String.IsNullOrEmpty(Error);

    public void FromRaw(RawMetadataJson raw)
    {
         Fields = String.Join("\t", raw.Fields.Select(f => $"{f.Name}\b{f.Type}\b{f.Length}"));
    }

    public RawMetadataJson ToRaw()
    {
        static RawField FromString(String ss)
        {
            var x = ss.Split('\b');
            if (x.Length != 3)
                return new RawField();

            return new RawField()
            {
                Name = x[0],
                Type = (RawFieldType) Enum.Parse(typeof(RawFieldType), x[1], true),
                Length = Convert.ToInt32(x[2])
            };
        }
        var arr = Fields.Split('\t').Select(FromString);
        var rm = new RawMetadataJson()
        {
            Fields = arr.ToArray()
        };
        return rm;
    }
    internal static MetadataJson Empty(String dir)
    {
        return new MetadataJson()
        {
            Directory = dir,
            Error = "metadata.json is empty"
        };
    }

    internal static MetadataJson Parse(AdditionalText addText, CancellationToken cancellationToken)
    {
        var ms = new MetadataJson() 
        {
            Directory = DirectoryFilter.DirectoryName(addText.Path)
        };

        var json = addText.GetText(cancellationToken)?.ToString();

        if (String.IsNullOrEmpty(json))
        {
            ms.Error = "metadata.json is empty";
            return ms;
        }

        try
        {
            var rawMeta = JsonConvert.DeserializeObject<RawMetadataJson>(json!, JsonSerializerHelpers.CamelCaseSettings);
            if (rawMeta == null)
            {
                ms.Error = "metadata.json deserialize fail";
                return ms;
            }
            ms.FromRaw(rawMeta);
            return ms;
        }
        catch (Exception ex)
        {
            ms.Error = ex.Message;
            return ms;
        }
    }
}

