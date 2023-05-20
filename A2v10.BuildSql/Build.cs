// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace A2v10.Sql.MSBuild;

public class Build : Task
{
	[Required]
	public String? ProjectDir { get; set; }

	public override Boolean Execute()
	{
		Log.LogMessage(MessageImportance.High, "Build sql started..");

		if (ProjectDir == null)
		{
			return false;
		}

		Log.LogMessage(MessageImportance.High, $"Project dir: {ProjectDir}");

		var sqlJsonPath = Path.GetFullPath(Path.Combine(ProjectDir, "sql.json"));
		if (!File.Exists(sqlJsonPath))
		{
			return true; // success
		}

		try
		{
			var sqlBuilder = new SqlFileBuilder(ProjectDir, Log);
			sqlBuilder.Process();
			Log.LogMessage(MessageImportance.High, "Build sql completed");
			return true;
		} 
		catch (Exception ex)
		{
			Log.LogError($"Error: {ex.Message}");
			return false; // fail
		}
	}
}