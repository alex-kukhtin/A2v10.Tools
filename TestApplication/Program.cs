
using MainApp;
using MainApp.Catalog;

using Newtonsoft.Json;

using System.Dynamic;

namespace TestApplication;

internal class Program
{
    static async Task Main(string[] args)
    {
        var str = "{\"Id\": 244, \"Name\":\"Agent Name\", \"Code\":\"100\"}";
        var eo = JsonConvert.DeserializeObject<ExpandoObject>(str); 

        var p = new MainApp.ElementProvider();

        var elem = p.CreateElement("catalog/agent", eo);
        if (elem is IClrCatalogElement clrElem)
        {
            if (clrElem.BeforeSave != null)
                await clrElem.BeforeSave();
        }

        Console.WriteLine(elem.ToString());
    }
}
