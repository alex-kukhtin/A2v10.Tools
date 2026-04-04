// Copyright © 2023-2026 Oleksandr Kukhtin. All rights reserved.

using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace A2v10.BuildSql;

internal interface ISqlLogger
{
    void LogMessage(MessageImportance importance, String message);
}

internal class SqlMsBuildLogger(TaskLoggingHelper log) : ISqlLogger
{
    public void LogMessage(MessageImportance importance, String message) =>
        log.LogMessage(importance, message);
}

internal class SqlConsoleLogger : ISqlLogger
{
    public void LogMessage(MessageImportance importance, String message) =>
        Console.WriteLine(message);
}