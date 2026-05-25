using System.Dynamic;

namespace MainApp;

using A2v10.App.Infrastructure;
using A2v10.Module.Infrastructure.Impl;

public interface IClrEventSource
{
    Func<Task<Boolean>>? BeforeSave { get; }
    Func<Task>? AfterSave { get; }
}

public interface IClrCatalogElement : IClrElement, IClrEventSource
{
}

// Базовий клас для каталогів - має бути в бібліотеці спільних класів
public class CatalogBase<T> : IClrCatalogElement where T : struct
{
    protected ExpandoObject? _source;
    public CatalogBase(IServiceProvider service)
    {
    }

    public CatalogBase(IServiceProvider service, ExpandoObject? src)
    {
        if (src == null)
            return;
        var d = (IDictionary<String, Object?>)src;
        //Id = d.TryGetId<T>("Id");
        //Name = d.TryGetString("Name");
    }

    public T Id { get; init; }
    public String? Name { get; set; }


    protected virtual void Init() { }
    public Func<Task<Boolean>>? BeforeSave { get; protected set; }
    public Func<Task>? AfterSave { get; protected set; }

    public virtual void ToExpando()
    {
    }
}
