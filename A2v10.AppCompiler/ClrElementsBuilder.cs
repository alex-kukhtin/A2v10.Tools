// Copyright © 2025-2026 Oleksandr Kukhtin. All rights reserved.

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
        foreach (var (file, assembly) in items)
        {
            var (path, content) = file;
            nspace = assembly;

            var fullPath = Path.GetDirectoryName(path)!;
            var split = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var ix = split.IndexOf(nspace);
            var relPath = split.Skip(ix + 1);
            var fileName = String.Join("_", relPath);
            var schema = relPath.FirstOrDefault().ToSchemaName();

            if (content == null)
                continue;

            var metajson = JsonConvert.DeserializeObject<MetadataJson>(content.ToString(), JsonSerializerHelpers.CamelCaseSettings);
            if (metajson != null && metajson.UseServerEvents)
            {
                var sourceText = SourceText.From(CreateText(metajson, nspace, schema), Encoding.UTF8);
                context.AddSource($"{fileName}.g.cs", sourceText);
                providedItems.Add($"[\"{String.Join("/", relPath)}\"] = (model, serviceProvider)  => new {nspace}.{schema}.{metajson.Name}(serviceProvider, model.Get<ExpandoObject>(\"{metajson.Name}\"))");
            }
        }

        if (providedItems.Count == 0)
            return;
        var providerSourceText = SourceText.From(CreateProviderModule(providedItems, nspace), Encoding.UTF8);
        context.AddSource($"_provider.g.cs", providerSourceText);
    }

    static String CreateText(MetadataJson meta, String nspace, String schema)
    {
        IEnumerable<String> props()
        {
            foreach (var f in meta.Fields)
            {
                var len = f.Length > 0 ? $"[MaxLength({f.Length})]\r\n\t" : String.Empty;
                var rq = f.Required ? "[Required]\r\n\t" : String.Empty;
                yield return $"{rq}{len}public {f.Type.ToPropertyType()} {f.Name} {{ get; set; }}";
            }
        }

        IEnumerable<String> assignProps()
        {
            foreach (var f in meta.Fields)
                yield return $"{f.Name} = d.TryGetString(\"{f.Name}\");";
        }

        IEnumerable<String> expProps()
        {
            foreach (var f in meta.Fields)
                yield return $"d[nameof({f.Name})] = {f.Name};";
        }

        var idType = "Int64"; // TODO:

        var code = 
$$""""
{{Constants.AUTO_GENERATED}}

#nullable enable

using System.Dynamic;
using System.ComponentModel.DataAnnotations;

using A2v10.App.Infrastructure;


namespace {{nspace}}.{{schema}};

public partial class {{meta.Name}} : {{schema}}Base<{{idType}}>
{
    {{String.Join("\r\n\t", props())}}

    public {{meta.Name}}(IServiceProvider serviceProvider) : base(serviceProvider) 
    {
        Init();
    }

    public {{meta.Name}}(IServiceProvider serviceProvider, ExpandoObject? src) : base(serviceProvider, src)
    {
        if (src != null) {
            var d = (IDictionary<String, Object?>)src;
            {{String.Join("\r\n\t\t\t", assignProps())}}
        }
        Init();
    }   

    public override void ToExpando()
    {
        base.ToExpando();
        if (_source == null)
            return;
        var d = (IDictionary<String, Object?>) _source;
        {{String.Join("\r\n\t\t", expProps())}}
    }
}
"""";

        return code;
    }


    static String CreateProviderModule(IReadOnlyList<String> items, String nspace)
    {
        var dictItems = String.Join(",\r\n\t\t", items);
        var code =
$$""""
{{Constants.AUTO_GENERATED}}    

#nullable enable

using System.Dynamic;

using A2v10.App.Infrastructure;

namespace {{nspace}};

public static class StartupClr
{
    private static readonly Dictionary<String, Func<ExpandoObject, IServiceProvider, IClrElement>> _elemMap = 
        new Dictionary<String, Func<ExpandoObject, IServiceProvider, IClrElement>>()
    {
        {{dictItems}}
    };

    public static void Register(AppMetadataClrOptions opts)
    {
        opts.AddRange(_elemMap);
    }
}

"""";
        return code;
    }
}
