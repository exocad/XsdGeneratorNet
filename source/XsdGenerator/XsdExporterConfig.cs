using System;
using System.Text;

namespace XsdGenerator;

public sealed record XsdExporterConfig
{
    public WriteLine WriteLine { get; init; } = ConsoleWriter.WriteLine;

    public StringComparison TypeNameStringComparison { get; init; } = StringComparison.Ordinal;

    public bool SkipSchemaValidation { get; init; }

    public string OutputDirectory { get; init; } = Environment.CurrentDirectory;

    public Encoding OutputFileEncoding { get; init; } = new UTF8Encoding(
        encoderShouldEmitUTF8Identifier: true);

    public static XsdExporterConfig Default { get; } = new();
}
