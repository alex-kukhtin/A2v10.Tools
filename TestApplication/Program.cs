
using A2v10.App.Infrastructure;
using A2v10.BuildSql;
using MainApp;
using MainApp.Catalog;

using Newtonsoft.Json;

using System.Dynamic;

namespace TestApplication;

internal class Program
{
    static async Task Main(string[] args)
    {
        /*
        var str = "{\"Agent\": {\"Id\": 244, \"Name\":\"Agent Name\", \"Code\":\"100\"}}";
        var eo = JsonConvert.DeserializeObject<ExpandoObject>(str)
            ?? throw new InvalidOperationException("Deserialization failed");   

        var appMetaOpts = new AppMetadataClrOptions();
        MainApp.StartupClr.Register(appMetaOpts);
        appMetaOpts.Map.TryGetValue("catalog/agent", out var func);
        var elem = func(eo, null);

        if (elem is IClrElementEventSource clrElem)
        {
            if (clrElem.BeforeSave != null)
                await clrElem.BeforeSave(new CancelToken());
        }

        Console.WriteLine(elem.ToString());
        */
        //TestBuilder.Build(@"C:\A2v10_Net6\A2v10.Standard.Modules\MainApp");
    }
}
