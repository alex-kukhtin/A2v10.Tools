// Copyright © 2025 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using Newtonsoft.Json;

namespace A2v10.AppCompiler;

internal class ClrElementsBuilder
{
    public static void Build(SourceProductionContext context, ImmutableArray<((String path, SourceText? content) file, String assembly)> items)
    {
        var providedItems = new List<String>();
        var nspace = String.Empty;  
        foreach (var pair in items)
        {
            var file = pair.file;
            nspace = pair.assembly;

            var fullPath = Path.GetDirectoryName(file.path)!;
            var split = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var ix = split.IndexOf(nspace);
            var relPath = split.Skip(ix + 1);
            var fileName = String.Join("_", relPath);
            var schema = relPath.FirstOrDefault().ToSchemaName();

            if (file.content == null)
                continue;

            var metajson = JsonConvert.DeserializeObject<MetadataJson>(file.content.ToString(), JsonSerializerHelpers.CamelCaseSettings);
            if (metajson != null)
            {
                var sourceText = SourceText.From(CreateText(metajson, nspace, schema), Encoding.UTF8);
                context.AddSource($"{fileName}.g.cs", sourceText);
                providedItems.Add($"[\"{String.Join("/", relPath)}\"] = eo => new {nspace}.{schema}.{metajson.Name}(eo)");
            }
        }

        if (providedItems.Count == 0)
            return;
        var providerSourceText = SourceText.From(CreateProviderModue(providedItems, nspace), Encoding.UTF8);
        context.AddSource($"_provider.g.cs", providerSourceText);
    }

    static String CreateText(MetadataJson meta, String nspace, String schema)
    {
        IEnumerable<String> props()
        {
            foreach (var f in meta.Fields)
            {
                var len = f.Length > 0 ? $"[MaxLength({f.Length})]\r\n\t" : String.Empty;
                yield return $"{len}public {f.Type.ToPropertyType()} {f.Name} {{ get; set; }}";
            }
        }

        IEnumerable<String> assignProps()
        {
            foreach (var f in meta.Fields)
            {
                yield return $"{f.Name} = d.TryGetString(\"{f.Name}\");";
            }
        }

        var idType = "Int64"; // TODO:

        var code = 
$$""""
{{Constants.AUTO_GENERATED}}

#nullable enable

using System.Dynamic;
using System.ComponentModel.DataAnnotations;

using A2v10.Module.Infrastructure.Impl;

namespace {{nspace}}.{{schema}};

public partial class {{meta.Name}} : {{schema}}Base<{{idType}}>
{
    {{String.Join("\r\n\t", props())}}

    public {{meta.Name}}() : base() {}

    public {{meta.Name}}(ExpandoObject src) : base(src)
    {
        var d = (IDictionary<String, Object?>)src;
        {{String.Join("\r\n\t\t", assignProps())}}
        Init();
    }   
}
"""";

        return code;
    }


    static String CreateProviderModue(IReadOnlyList<String> items, String nspace)
    {
        var dictItems = String.Join(",\r\n\t\t", items);
        var code =
$$""""
{{Constants.AUTO_GENERATED}}    

#nullable enable

using System.Dynamic;

namespace {{nspace}};

public class ElementProvider
{
    private static IReadOnlyDictionary<String, Func<ExpandoObject, IClrElement>> _elemMap =
    new Dictionary<String, Func<ExpandoObject, IClrElement>>(StringComparer.OrdinalIgnoreCase)
    {
        {{dictItems}}
    }.AsReadOnly();

    public IClrElement CreateElement(String typeName, ExpandoObject eo)
    {
        if (_elemMap.TryGetValue(typeName, out var createElem))
            return createElem(eo);
        throw new InvalidOperationException($"Unknown type name: {typeName}");
    }   
}

"""";
        return code;
    }
}
