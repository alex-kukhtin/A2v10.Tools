using System.Dynamic;

namespace MainApp.catalog.agent;

// Цей клас має бути згенеровано автоматично.
public partial class Agent : CatalogBase<Int64>
{
    public String? Code { get; set; }

    public static Agent FromExpando(ExpandoObject agent)
    {
        return new Agent();
    }
}

// Цей клас має бути згенеровано автоматично.
public class ElementProvider
{
    private static IReadOnlyDictionary<String, Func<ExpandoObject, IClrCatalogElement>> _elemMap =
        new Dictionary<String, Func<ExpandoObject, IClrCatalogElement>>(StringComparer.OrdinalIgnoreCase)
        {
            ["catalog/agent"] = eo => Agent.FromExpando(eo)
        }.AsReadOnly();

    public IClrCatalogElement CreateCatalogElement(String typeName, ExpandoObject eo)
    {
        if (_elemMap.TryGetValue(typeName, out var createElem))
            return createElem(eo);
        throw new InvalidOperationException($"Unknown type name: {typeName}");
    }   
}
