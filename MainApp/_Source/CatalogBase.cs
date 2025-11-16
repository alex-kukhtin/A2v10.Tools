namespace MainApp;

public interface IClrCatalogElement
{
    Func<Task<Boolean>>? BeforeSave { get; }
    Func<Task>? AfterSave { get; }
}

// Базовий клас для каталогів - має бути в бібліотеці спільних класів
public class CatalogBase<T> : IClrCatalogElement where T : struct
{
    public T Id { get; init; }
    public String? Name { get; set; }

    public Func<Task<Boolean>>? BeforeSave { get; init; }
    public Func<Task>? AfterSave { get; init; }
}
