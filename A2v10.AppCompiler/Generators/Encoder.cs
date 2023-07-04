// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Linq;

namespace A2v10.AppCompiler;

internal static class Encoder
{
    internal static String Encode(String text)
    {
        using var mstarget = new MemoryStream();
        var src = Encoding.UTF8.GetBytes(text);
        using (var ds = new DeflateStream(mstarget, CompressionLevel.Optimal))
        {
            ds.Write(src, 0, src.Length);
        }
        return BitConverter.ToString(mstarget.ToArray());
        //return Convert.ToBase64String(mstarget.ToArray());
    }

    internal static String EncodeBytes(String text)
    {
        using var mstarget = new MemoryStream();
        var src = Encoding.UTF8.GetBytes(text);
        using (var ds = new DeflateStream(mstarget, CompressionLevel.Optimal))
        {
            ds.Write(src, 0, src.Length);
        }
        var bytes = String.Join(",", mstarget.ToArray().Select(b => $"0x{b:x}"));
        return $"new byte[] {{ {bytes} }}";
    }
}
