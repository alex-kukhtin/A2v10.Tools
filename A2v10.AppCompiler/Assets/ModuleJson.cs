// Copyright © 2023-2025 Oleksandr Kukhtin. All rights reserved.

using System;
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

    public void ReplaceMacros(StringBuilder sb)
    {
        sb.Replace("$(id)", Id);
        sb.Replace("$(name)", Name);
        sb.Replace("$(authors)", Authors);
        sb.Replace("$(company)", Company);
        sb.Replace("$(description)", Description);
        sb.Replace("$(copyright)", Copyright);
    }
}

internal class ModuleJson
{
	public static ModuleDef? Load(String json)
	{
		try
		{
			return JsonConvert.DeserializeObject<ModuleDef>(json, JsonSerializerHelpers.CamelCaseSettings);
		}
		catch (Exception)
		{ 
			return null; 
		}
	}
}
