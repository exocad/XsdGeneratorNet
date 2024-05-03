using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;

namespace XsdGenerator;

/// <summary>
/// The <see cref="XsdExporter"/> class allows generating XSD files from C# types exported from a .NET Core assembly.
/// </summary>
public sealed class XsdExporter
{
    private readonly XsdExporterConfig _config;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="XsdExporterConfig"/> class.
    /// </summary>
    /// <param name="config">The configuration for this exporter instance. See <see cref="XsdExporterConfig"/> for details.</param>
    public XsdExporter(XsdExporterConfig config) => _config = config;

    /// <summary>
    /// Loads the given assemblies and tries to export all types that are listed in the <paramref name="typeNames"/>
    /// collection. Each type found will be exported to a separate XSD file.
    /// 
    /// Calling this method multiples times will override previously written XSD files. Consider creating a new
    /// instance with a different output directory if multiple invocations are required.
    /// </summary>
    /// <param name="assemblyPaths">
    /// The filenames of the assemblies to load.
    /// </param>
    /// <param name="typeNames">
    /// The typenames to generate an XSD schema for. Appending a <c>.*</c> to a typename will include all types
    /// whose full name starts with the given string.
    /// </param>
    /// <returns>
    /// An <see cref="XsdExportResult"/> containing the paths to the generated XSD files and schema validation
    /// warnings, if there are any.
    /// </returns>
    public XsdExportResult ExportSchemas(IReadOnlyList<string> assemblyPaths, IReadOnlyList<string> typeNames)
    {
        var schemas = new XmlSchemaContainer();
        var result = new XsdExportResult();

        foreach (var assemblyPath in assemblyPaths)
        {
            var typeList = new TypeList(assemblyPath, typeNames, _config);

            if (typeList.ExportableTypes.Count == 0)
            {
                WriteLine(Priority.Normal, $"No exportable types were found in '{assemblyPath}'.");
                continue;
            }

            foreach (var type in typeList.ExportableTypes)
            {
                schemas.GenerateSchema(type);
            }

            schemas.Compile((_, e) => OnValidateSchema(result, e), fullCompile: false);
        }

        foreach (var (schema, type, index) in schemas.EnumerateSchemas())
        {
            var filename = GetFilename(schema, index, schemas);

            using var writer = CreateTextWriter(filename);

            WriteLine(Priority.Normal, type is not null 
                ? $"Generating file for exported type '{type}': {filename}"
                : $"Generating file: {filename}");

            result.AddExportedSchema(filename, schema, type);
            schema.Write(writer);
        }

        return result;
    }

    private string GetFilename(XmlSchema schema, int index, XmlSchemaContainer container)
    {
        return $"output{index}.xsd";
    }

    private TextWriter CreateTextWriter(string filename)
    {
        var path = Path.Join(_config.OutputDirectory, filename);

        return new StreamWriter(path, append: false, _config.OutputFileEncoding);
    }

    private void WriteLine(Priority priority, string message)
    {
        _config.WriteLine(priority, message);
    }

    private void OnValidateSchema(XsdExportResult result, ValidationEventArgs e)
    {
        if (_config.SkipSchemaValidation)
        {
            return;
        }

        result.AddValidationWarning(e.Exception);

        var priority = e.Severity == XmlSeverityType.Error ? Priority.Error : Priority.Warning;

        if (e.Exception is { LineNumber: 0, LinePosition: 0 })
        {
            WriteLine(priority, $"Schema validation {priority}: {e.Exception.Message}");
        }
        else
        {
            var line = e.Exception.LineNumber;
            var position = e.Exception.LinePosition;

            WriteLine(priority, $"Schema validation {priority} at ({line}, {position}): {e.Exception.Message}");
        }
    }
}