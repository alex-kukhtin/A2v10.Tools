using System.ComponentModel.DataAnnotations;
using System.Dynamic;

using A2v10.Module.Infrastructure.Impl;
using A2v10.App.Infrastructure;
using System;
using System.Collections.Generic;

namespace MainApp.Catalog;

// Цей клас має бути згенеровано автоматично.
public partial class AgentXX : CatalogBase<Int64>
{
    public AgentXX(IServiceProvider service, ExpandoObject src) : base(service, src)  
    {
        var d = (IDictionary<String, Object?>)src;
        //Code = d.TryGetString("Code");
    }

    [MaxLength(255)]
    public String? Code { get; set; }
}

// Цей клас має бути згенеровано автоматично.
public class ElementProvider2222
{
    private static IReadOnlyDictionary<String, Func<ExpandoObject, IClrElement>> _elemMap =
    new Dictionary<String, Func<ExpandoObject, IClrElement>>(StringComparer.OrdinalIgnoreCase)
    {
        ["catalog/agent"] = eo => new Agent(null, eo)
    }.AsReadOnly();

    public IClrElement CreateElement(String typeName, ExpandoObject eo)
    {
        if (_elemMap.TryGetValue(typeName, out var createElem))
            return createElem(eo);
        throw new InvalidOperationException($"Unknown type name: {typeName}");
    }   
}
