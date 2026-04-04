// Copyright © 2023-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Build.Framework;

using A2v10.BuildSql;

namespace A2v10.Sql.MSBuild;

internal class SqlFileBuilder
{
	private readonly String _path;
	private readonly ISqlLogger _log;

	internal SqlFileBuilder(String path, ISqlLogger log)
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
				foreach (var resolvedPath in ResolveInputFiles(f))
				{
                    var relativePath = resolvedPath.StartsWith(_path)
                        ? resolvedPath.Substring(_path.Length).TrimStart(Path.DirectorySeparatorChar)
                        : resolvedPath;
                    relativePath = relativePath.Replace('\\', '/');
                    _log.LogMessage(MessageImportance.High, $"\t{relativePath}"); 
					var inputText = new StringBuilder(File.ReadAllText(resolvedPath));
					sw.Write(inputText.ToString());
					sw.WriteLine();
				}
			}
		}
		finally
		{
			fw?.Close();
		}
	}

    IEnumerable<String> ResolveInputFiles(String pattern)
    {
        const String recursiveMarker = "/**/";

        int markerIndex = pattern.IndexOf(recursiveMarker);
        if (markerIndex < 0)
        {
            // general
            yield return Path.Combine(_path, pattern);
            yield break;
        }

        String baseDir = pattern.Substring(0, markerIndex);
        String fileName = pattern.Substring(markerIndex + recursiveMarker.Length);

        String searchRoot = Path.Combine(_path, baseDir);
        if (!Directory.Exists(searchRoot))
            yield break;

        foreach (var file in Directory.EnumerateFiles(searchRoot, fileName, SearchOption.AllDirectories))
        {
            yield return file;
        }
    }
}
