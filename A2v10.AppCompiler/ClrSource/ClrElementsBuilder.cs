// Copyright © 2025-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace A2v10.AppCompiler;

/* TODO:
 * 1. Relative Path!
 * 2. Base Element
 */
internal class ClrElementsBuilder
{
    public static void Build(SourceProductionContext context, ImmutableArray<(ClassModel model, MetadataJson meta)> items)
    {
        if (items.Length == 0)
            return;

        var allElems = items.Select((itm) => 
            $"""["{itm.model.Directory}"] = (model, serviceProvider) => new {itm.model.Namespace}.{itm.model.Name}(serviceProvider, model.Get<ExpandoObject>("{itm.model.Name}"))""");

        context.AddSource("register.g.cs", CreateProviderModule(allElems));

        foreach (var itm in items)
        {
            var meta = itm.meta.ToRaw();
            var cls = itm.model;
            var fileName = $"_{cls.Name.ToLowerInvariant()}.g.cs";
            context.AddSource(fileName, CreateText(cls, meta));
        }
    }

    static String CreateText(ClassModel cls, RawMetadataJson meta)
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
                yield return $"{f.Name} = d.TryGetString(\"{f.Name}\");";
        }

        IEnumerable<String> expProps()
        {
            foreach (var f in meta.Fields)
                yield return $"d[nameof({f.Name})] = {f.Name};";
        }

        var code =
        $$""""
        {{Constants.AUTO_GENERATED}}

        #nullable enable

        using System.Dynamic;
        using System.ComponentModel.DataAnnotations;

        using A2v10.App.Infrastructure;

        namespace {{cls.Namespace}};

        public partial class {{cls.Name}} : CatalogBase<Int64>
        {
            {{String.Join("\r\n\t", props())}}

            public {{cls.Name}}(IServiceProvider serviceProvider) : base(serviceProvider) 
            {
                Init();
            }

            public {{cls.Name}}(IServiceProvider serviceProvider, ExpandoObject? src) : base(serviceProvider, src)
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


    static String CreateProviderModule(IEnumerable<String> items)
    {
        var dictItems = String.Join(",\r\n\t\t", items);
        var code =
        $$""""
        {{Constants.AUTO_GENERATED}}    

        #nullable enable

        using System.Dynamic;
        using A2v10.App.Infrastructure;

        namespace Generated;

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
