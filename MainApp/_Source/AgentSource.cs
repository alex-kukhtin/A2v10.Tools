using System.ComponentModel.DataAnnotations;
using System.Dynamic;

using A2v10.Module.Infrastructure.Impl;
using A2v10.App.Infrastructure;
using System;
using System.Collections.Generic;

namespace MainApp;

// Цей клас має бути згенеровано автоматично.
public partial class AgentXX : CatalogBase<Int64>
{
    public AgentXX(IServiceProvider service, ExpandoObject src) : base(service, src)  
    {
        //var d = (IDictionary<String, Object?>)src;
        //Code = d.TryGetString("Code");
    }

    [MaxLength(255)]
    public String? Code { get; set; }
}

