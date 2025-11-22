
using MainApp;
using MainApp.Catalog;

using Newtonsoft.Json;

using System.Dynamic;

namespace TestApplication;

internal class Program
{
    static async Task Main(string[] args)
    {
        var str = "{\"Agent\": {\"Id\": 244, \"Name\":\"Agent Name\", \"Code\":\"100\"}}";
        var eo = JsonConvert.DeserializeObject<ExpandoObject>(str)
            ?? throw new InvalidOperationException("Deserialization failed");   

        var p = new MainApp.ElementProvider();

        var elem = p.CreateElement("catalog/agent", eo);
        if (elem is IClrEventSource clrElem)
        {
            if (clrElem.BeforeSave != null)
                await clrElem.BeforeSave();
        }

        Console.WriteLine(elem.ToString());
    }
}
