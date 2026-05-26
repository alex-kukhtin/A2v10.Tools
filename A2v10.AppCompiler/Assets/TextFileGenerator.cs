// Copyright © 2022-2026 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.Text;

namespace A2v10.AppCompiler;

internal record AssetInfo(String RelativePath, String Identifier);

internal class TextFileGenerator
{
    internal static AssetInfo GetFileInfo(String path, String basePath)
    {
        String normPath = path.Replace('\\', '/');
        String normBase = basePath.Replace('\\', '/').TrimEnd('/');

        String relative;
        if (normPath.StartsWith(normBase + "/", StringComparison.OrdinalIgnoreCase))
            relative = normPath.Substring(normBase.Length + 1);
        else
        {
            relative = normPath.TrimStart('/');
        }

        Char[] chars = [.. relative.Select(static c =>
            (Char.IsLetterOrDigit(c) || c == '_') ? c : '_')];
        String ider = "_" + new String(chars);

        return new AssetInfo(relative, ider);
    }


    public static SourceText GetSource(String basePath, ImmutableArray<(String path, String content)> items, String ns)
    {
        var sb = new StringBuilder(HEADER.Replace("$namespace$", ns));
        var sbDict = new StringBuilder();
        var sbBody = new StringBuilder();
        foreach (var (path, content) in items)
        {
            var fi = GetFileInfo(path, basePath);
            sbDict.AppendLine(MapEntry(fi));
            sbBody.AppendLine(MapBody(fi, content));
        };
        sb.AppendLine(sbDict.ToString());
        sb.AppendLine("\t};");

        sb.AppendLine(sbBody.ToString());
        sb.AppendLine("}"); // end class;
        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }

    static String MapEntry(AssetInfo file)
    {
        return $"\t\t[\"{file.RelativePath}\"] = {file.Identifier},";
    }

    static String MapBody(AssetInfo file, String? text)
    {
        String statement = "Array.Empty<Byte>()";
        if (!String.IsNullOrEmpty(text))
            statement = Encoder.EncodeBytes(text!);
        return $"\tstatic Byte[] {file.Identifier}() => {statement};";
    }

    private const String HEADER = Constants.AUTO_GENERATED + """
    
    #nullable enable
    
    using System;
    using System.IO;
    using System.Collections.Generic;

    using A2v10.Module.Infrastructure.Impl;

    namespace $namespace$;

    public class TextFilesContainer 
    {
        public String? Get(String path) {
            if (_textMap.TryGetValue(path, out var textFile))
                return DecodeHelpers.Decode(textFile());
            return null;
        }

        public Stream? GetStream(String path) {
            if (_textMap.TryGetValue(path, out var textFile))
                return DecodeHelpers.DecodeStream(textFile());
            return null;
        }

        public IEnumerable<String> EnumerateFiles(String path, String searchPattern)
        {
            foreach (var k in _textMap.Keys) {
                if ((path == null || k.StartsWith(path)) && k.EndsWith(searchPattern))
                    yield return k;
            }
        }

        public Boolean Exists(String path)
        {
            return _textMap.ContainsKey(path);
        }

        private readonly Dictionary<String, Func<Byte[]>> _textMap = new()
        {

    """;
}
