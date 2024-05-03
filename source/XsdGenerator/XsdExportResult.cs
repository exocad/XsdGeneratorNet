using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace XsdGenerator;

/// <summary>
/// The <see cref="XsdExportResult"/> class contains a summary of the <see cref="XsdExporter.ExportSchemas(IReadOnlyList{string}, IReadOnlyList{string})"/> operation.
/// </summary>
public sealed class XsdExportResult
{
    private readonly List<XmlSchemaException> _validationWarnings = new();
    private readonly List<XsdExportedSchema> _schemas = new();

    /// <summary>
    /// Gets a value indicating whether any schema validation warnings occurred during the export.
    /// </summary>
    public bool HasValidationWarnings => _validationWarnings.Count > 0;

    /// <summary>
    /// Gets a collection of all validation warnings or errors.
    /// </summary>
    public IReadOnlyList<XmlSchemaException> ValidationWarnings => _validationWarnings;

    /// <summary>
    /// Gets a collection of all exported schemas.
    /// </summary>
    public IReadOnlyList<XsdExportedSchema> ExportedSchemas => _schemas;

    internal void AddValidationWarning(XmlSchemaException ex) => _validationWarnings.Add(ex);

    internal void AddExportedSchema(string filename, XmlSchema schema, Type? type) => _schemas.Add(new XsdExportedSchema(filename, schema, type));
}
