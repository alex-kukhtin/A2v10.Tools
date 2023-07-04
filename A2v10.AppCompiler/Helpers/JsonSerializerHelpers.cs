// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace A2v10.AppCompiler;

internal static class JsonSerializerHelpers
{
	public static JsonSerializerSettings CamelCaseSettings = new()
	{
		ContractResolver = new DefaultContractResolver()
		{
			NamingStrategy = new CamelCaseNamingStrategy()
		}

	};
}
