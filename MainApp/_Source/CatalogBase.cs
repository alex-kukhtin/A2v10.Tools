using System.Dynamic;

namespace MainApp;

using A2v10.Module.Infrastructure.Impl;

public interface IClrElement22
{
}
public interface IClrEventSource22
{
    Func<Task<Boolean>>? BeforeSave { get; }
    Func<Task>? AfterSave { get; }
}

public interface IClrCatalogElement22 : IClrElement22, IClrEventSource22
{
}

// Базовий клас для каталогів - має бути в бібліотеці спільних класів
public class CatalogBase22<T> : IClrCatalogElement22 where T : struct
{
    public CatalogBase22()
    {
    }

    public CatalogBase22(ExpandoObject? src)
    {
        if (src == null)
            return;
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
