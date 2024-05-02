using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;

namespace XsdGenerator;

public sealed class XsdExporter
{
    private readonly XsdExporterConfig _config;
    
    public XsdExporter(XsdExporterConfig config)
    {
        _config = config;
    }

    public XsdExportResult ExportSchemas(IReadOnlyList<string> assemblyPaths, IReadOnlyList<string> typeNames)
    {
        var schemas = new XmlSchemaContainer();
        var result = new XsdExportResult();

        foreach (var assemblyPath in assemblyPaths)
        {
            var typeList = new TypeList(assemblyPath, typeNames, _config);

            if (typeList.ExportableTypes.Count == 0)
            {
                WriteLine(Priority.Warning, $"No exportable types were found in '{assemblyPath}'.");
                continue;
            }

            foreach (var type in typeList.ExportableTypes)
            {
                schemas.GenerateMapping(type);
            }

            schemas.Compile((_, e) => OnValidateSchema(result, e), fullCompile: false);
        }

        for (var index = 0; index < schemas.Count; ++index)
        {
            var schema = schemas[index];
            var filename = GetFilename(schema, index, schemas, out var type);

            using var writer = CreateTextWriter(filename);

            result.AddExportedItem(filename, type);
            schema.Write(writer);
        }

        return result;
    }

    private string GetFilename(XmlSchema schema, int index, XmlSchemaContainer container, out Type? type)
    {
        var filename = $"output{index}.xsd";

        if (container.TryGetAssociatedType(schema, out type))
        {
            WriteLine(Priority.Normal, $"Generating file for exported type '{type}': {filename}");
        }

        return filename;
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

        result.AddViolationWarning(e.Exception);

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