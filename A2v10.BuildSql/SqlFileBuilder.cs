// Copyright © 2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using A2v10.BuildSql;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace A2v10.Sql.MSBuild;

internal class SqlFileBuilder
{
	private readonly String _path;
	private readonly TaskLoggingHelper _log;

	public SqlFileBuilder(String path, TaskLoggingHelper log)
	{
		_path = path;
		_log = log;
	}

	public void Process()
	{
		String jsonPath = Path.Combine(_path, "sql.json");
		if (!File.Exists(jsonPath))
		{
			return;
		}

		_log.LogMessage(MessageImportance.High, $"Processing {jsonPath}");

		String jsonText = File.ReadAllText(jsonPath);
		List<SqlFileItem> list = JSONParser.DeserializeObject<List<SqlFileItem>>(jsonText)
			?? throw new InvalidOperationException("invalid sql.json");

		foreach (var item in list)
		{
			ProcessOneItem(item);
		}
	}

	void ProcessOneItem(SqlFileItem item)
	{
		String outFilePath = Path.Combine(_path, item.OutputFile ?? 
			throw new InvalidOperationException("Output file is null"));

		String dirName = Path.GetDirectoryName(Path.GetFullPath(outFilePath))
			?? throw new InvalidOperationException("Invalid directory");
		if (!Directory.Exists(dirName))
			Directory.CreateDirectory(dirName);

		File.Delete(outFilePath);
		var nl = Environment.NewLine;
		FileStream? fw = null;
		try
		{
			fw = File.Open(outFilePath, FileMode.CreateNew, FileAccess.Write);
			_log.LogMessage(MessageImportance.High, $"Writing {item.OutputFile}");
			using var sw = new StreamWriter(fw, new UTF8Encoding(true));
			fw = null;
			sw.Write($"/* {item.OutputFile} */{nl}{nl}");
			foreach (var f in item.InputFiles)
			{
				var inputPath = Path.Combine(_path, f);
				_log.LogMessage(MessageImportance.High, $"\t{f}");
				var inputText = new StringBuilder(File.ReadAllText(inputPath));
				sw.Write(inputText.ToString());
				sw.WriteLine();
			}
		}
		finally
		{
			fw?.Close();
		}
	}
}
