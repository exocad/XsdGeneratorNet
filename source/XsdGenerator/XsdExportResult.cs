using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace XsdGenerator;

public sealed class XsdExportResult
{
    private readonly List<XmlSchemaException> _validationWarnings = new();
    private readonly List<XsdExportItem> _exportedItems = new();

    public bool HasValidationWarnings => _validationWarnings.Count > 0;

    public IReadOnlyList<XmlSchemaException> ValidationWarnings => _validationWarnings;

    public IReadOnlyList<XsdExportItem> ExportedItems => _exportedItems;

    internal void AddViolationWarning(XmlSchemaException ex) => _validationWarnings.Add(ex);

    internal void AddExportedItem(string filename, Type? type) => _exportedItems.Add(new XsdExportItem(filename, type));
}
