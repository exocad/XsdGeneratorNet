using System;
using System.Text;

namespace XsdGenerator;

/// <summary>
/// Configuration for the <see cref="XsdExporter"/> class.
/// </summary>
public sealed record XsdExporterConfig
{
    /// <summary>
    /// Gets the callback to use when writing log messages.
    /// </summary>
    public WriteLine WriteLine { get; init; } = ConsoleWriter.WriteLine;

    /// <summary>
    /// Gets the <see cref="StringComparison"/> method to use when comparing type names.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="StringComparison.Ordinal"/>.
    /// </remarks>
    public StringComparison TypeNameStringComparison { get; init; } = StringComparison.Ordinal;

    /// <summary>
    /// Gets a value indicating whether schema validation warnings shall be omitted and ignored.
    /// Event if enabled, errors will still cancel the export operation.
    /// </summary>
    /// <remarks>The default value is <c>false</c>.</remarks>
    public bool SkipSchemaValidation { get; init; }

    /// <summary>
    /// Gets the output directory for the XSD files.
    /// </summary>
    /// <remarks>The default value is <see cref="Environment.CurrentDirectory"/>.</remarks>
    public string OutputDirectory { get; init; } = Environment.CurrentDirectory;

    /// <summary>
    /// Gets the encoding to use when writing the XSD files.
    /// </summary>
    /// <remarks>The default value is UTF8 encoding with BOM.</remarks>
    public Encoding OutputFileEncoding { get; init; } = new UTF8Encoding(
        encoderShouldEmitUTF8Identifier: true);

    /// <summary>
    /// Gets a default instance of the <see cref="XsdExporterConfig"/> class.
    /// </summary>
    public static XsdExporterConfig Default { get; } = new();
}
