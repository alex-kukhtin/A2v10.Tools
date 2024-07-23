﻿// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;

using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;

namespace A2v10.AppCompiler;

/*
 * https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
 */

[Generator]
public class SourceGenerator : ISourceGenerator
{
	/*
	private static readonly DiagnosticDescriptor Info = 
		new  DiagnosticDescriptor(id: "CL1001",
			title: "MESSAGE",
			messageFormat: "{0}",
			category: "A2v10.AppCompiler",
			DiagnosticSeverity.Info,
			isEnabledByDefault: true);
	*/

	public void Execute(GeneratorExecutionContext context)
	{
		Debug.WriteLine("Start generator");
		var addFiles = context.AdditionalFiles;
		var nspace = context.Compilation.AssemblyName ?? String.Empty;
		if (addFiles.Length == 0)
			return;
		var file = addFiles[0];
		var path = Path.GetDirectoryName(file.Path);
		if (!Directory.Exists(path))
			throw new DirectoryNotFoundException(path);

		//context.ReportDiagnostic(Diagnostic.Create(Info, Location.None, $"AppPath: {path}"));
		Debug.WriteLine($"AppPath: {path}");

		/*
		 * Source from resources
        context.AddSource("helpers.g.cs", 
			SourceText.From(GetResource("A2v10.AppCompiler.Source.Helpers.cs", nspace), Encoding.UTF8));
		*/

		context.AddSource("textfiles.g.cs", TextFileGenerator.GetSource(path, nspace,
			[".json", ".js", ".txt", ".xaml", ".css", ".html"]));

		var sb = new StringBuilder(MAIN_CODE);
		sb.Replace("$(namespace)", nspace);
		sb.Replace("$(path)", Path.GetFileName(path));
		ModuleJson.ReplaceMacros(path, sb);
		context.AddSource("appcontainer.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
	}

	/*
	private String GetResource(String resourceName, String nspace)
	{
		using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
		using var sr = new StreamReader(rs);
		var sb = new StringBuilder(sr.ReadToEnd());
		sb.Replace("$(namespace)", nspace);
		return sb.ToString();
	}
	*/

	public void Initialize(GeneratorInitializationContext context)
	{
		//if (!Debugger.IsAttached)
		//	Debugger.Launch();
	}

	private const String MAIN_CODE = """
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a A2v10.App.Compiler.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

/* Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved. */

#nullable enable
using System;
using System.IO;
using System.Collections.Generic;

using A2v10.Module.Infrastructure;

namespace $(namespace);

public class AppContainer : IAppContainer 
{
	public Boolean IsLicensed => true;
	public Guid Id => _id;	
	public String ModuleName => "$(name)";
    public String? Authors => "$(authors)";
	public String? Company => "$(company)";
	public String? Description => "$(description)";
	public String? Copyright => "$(copyright)";

	private static readonly Guid _id = new Guid("$(id)");

	private readonly TextFilesContainer _textContainer = new();

	public String GetText(String path)
	{
		return _textContainer.Get(path);
	}

	public Stream GetStream(String path)
	{
		return _textContainer.GetStream(path);
	}

	public IEnumerable<String> EnumerateFiles(String path, String searchPattern)
	{
		return _textContainer.EnumerateFiles(path, searchPattern);
	}

	public Boolean FileExists(String path)
	{
		return _textContainer.Exists(path);
	}
}
""";
}
