// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace A2v10.AppCompiler;

internal record ModuleDef
{
	public String Id { get; set; } = String.Empty;
	public String Name { get; set; } = String.Empty;
	public String Authors { get; set; } = String.Empty;
	public String Company { get; set; } = String.Empty;
	public String Description { get; set; } = String.Empty;
	public String Copyright { get; set; } = String.Empty;
}

internal class ModuleJson
{
	public static void ReplaceMacros(String path, StringBuilder sb)
	{
		var fileName = Path.Combine(path, "module.json");
		var json = File.ReadAllText(fileName);
		var def = JsonConvert.DeserializeObject<ModuleDef>(json, JsonSerializerHelpers.CamelCaseSettings)
			?? throw new InvalidOperationException("Invalid module.json");
		sb.Replace("$(id)", def.Id);
		sb.Replace("$(name)", def.Name);
		sb.Replace("$(authors)", def.Authors);
		sb.Replace("$(company)", def.Company);
		sb.Replace("$(description)", def.Description);
		sb.Replace("$(copyright)", def.Copyright);
	}
}
