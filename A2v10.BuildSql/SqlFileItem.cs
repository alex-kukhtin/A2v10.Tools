// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;

namespace A2v10.Sql.MSBuild;

internal record SqlFileItem
{
	public String? Version { get; set; }
	public List<String> InputFiles { get; set; } = new List<String>();
	public String? OutputFile { get;set; }
}
