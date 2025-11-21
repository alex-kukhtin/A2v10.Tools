using System.Dynamic;

namespace MainApp;

using A2v10.Module.Infrastructure.Impl;

public interface IClrElement
{

}

public interface IClrCatalogElement : IClrElement
{
    Func<Task<Boolean>>? BeforeSave { get; }
    Func<Task>? AfterSave { get; }
}

// Базовий клас для каталогів - має бути в бібліотеці спільних класів
public class CatalogBase<T> : IClrCatalogElement where T : struct
{
    public CatalogBase()
    {
    }

    public CatalogBase(ExpandoObject src)
    {
        var d = (IDictionary<String, Object?>)src;
        Id = d.TryGetId<T>("Id");
        Name = d.TryGetString("Name");
    }

    public T Id { get; init; }
    public String? Name { get; set; }


    protected virtual void Init() { }
    public Func<Task<Boolean>>? BeforeSave { get; protected set; }
    public Func<Task>? AfterSave { get; protected set; }
}
