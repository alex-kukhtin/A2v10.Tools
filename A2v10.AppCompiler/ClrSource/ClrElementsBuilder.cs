// Copyright © 2025-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace A2v10.AppCompiler;

internal class ClrElementsBuilder
{
    static readonly DiagnosticDescriptor InvalidJsonDiagnostic = new(
        id: "A2AC003",
        title: "Invalid metadata.json",
        messageFormat: "metadata.json in '{0}' is invalid: {1}. Entity will not be registered.",
        category: "AppCompiler",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static void Build(SourceProductionContext context, ImmutableArray<(ClassModel model, MetadataJson meta)> items, String ns)
    {
        if (items.Length == 0)
            return;

        var allElems = items.Where(itm => itm.meta.IsValid).Select((itm) => 
            $"""["{DirectoryFilter.RelativeDirectory(itm.model.Directory, ns)}"] = (model, sp) => new {itm.model.Namespace}.{itm.model.Name}(sp, model.Get<ExpandoObject>("{itm.model.Name}"))""");

        context.AddSource("register.g.cs", CreateProviderModule(allElems, ns));

        foreach (var itm in items)
        {
            if (itm.meta.IsValid)
            {
                var meta = itm.meta.ToRaw();
                var cls = itm.model;
                var fileName = $"_{cls.Name.ToLowerInvariant()}.g.cs";
                context.AddSource(fileName, CreateText(cls, meta, ns));
            }
            else 
                context.ReportDiagnostic(Diagnostic.Create(InvalidJsonDiagnostic, Location.None, itm.meta.Directory, itm.meta.Error));
        }
    }

    static String CreateText(ClassModel cls, RawMetadataJson meta, String ns)
    {
        var baseClass = DirectoryFilter.RelativeDirectorySegment(cls.Directory, ns);
        baseClass = Char.ToUpperInvariant(baseClass[0]) + baseClass.Substring(1);

        IEnumerable<String> props()
        {
            foreach (var f in meta.Fields)
            {
                var len = f.Length > 0 ? $"[MaxLength({f.Length})]\n\t" : String.Empty;
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

        using System;
        using System.Dynamic;
        using System.Collections.Generic;
        using System.ComponentModel.DataAnnotations;

        using A2v10.App.Infrastructure;

        namespace {{cls.Namespace}};

        public partial class {{cls.Name}} : {{baseClass}}Base<Int64>
        {
            {{String.Join("\n\t", props())}}

            public {{cls.Name}}(IServiceProvider sp) : base(sp) 
            {
                Init();
            }

            public {{cls.Name}}(IServiceProvider sp, ExpandoObject? src) : base(sp, src)
            {
                if (src != null) {
                    var d = (IDictionary<String, Object?>)src;
                    {{String.Join("\n\t\t\t", assignProps())}}
                }
                Init();
            }   

            public override void ToExpando()
            {
                base.ToExpando();
                if (_source == null)
                    return;
                var d = (IDictionary<String, Object?>) _source;
                {{String.Join("\n\t\t", expProps())}}
            }
        }
        """";
        return code;
    }


    static String CreateProviderModule(IEnumerable<String> items, String ns)
    {
        var dictItems = String.Join(",\n\t\t", items);
        var code =
        $$""""
        {{Constants.AUTO_GENERATED}}    

        #nullable enable

        using System;
        using System.Dynamic;
        using System.Collections.Generic;

        using A2v10.App.Infrastructure;

        namespace {{ns}};

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
