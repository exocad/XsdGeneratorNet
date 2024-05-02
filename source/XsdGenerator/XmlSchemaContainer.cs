using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XsdGenerator;

internal sealed class XmlSchemaContainer : XmlSchemas
{
    private readonly Dictionary<XmlSchema, Type> _types = [];
    private readonly XmlReflectionImporter _importer = new();
    private readonly XmlSchemaExporter _exporter;
    private Type? _currentType;

    public XmlSchemaContainer()
    {
        _exporter = new XmlSchemaExporter(this);
    }

    public bool TryGetAssociatedType(XmlSchema schema, [MaybeNullWhen(false)] out Type type) =>
        _types.TryGetValue(schema, out type);

    public void GenerateMapping(Type type)
    {
        try
        {
            _currentType = type;
            _exporter.ExportTypeMapping(_importer.ImportTypeMapping(type));
        }
        finally
        {
            _currentType = null;
        }
    }

    #region Overrides
    protected override void OnInsert(int index, object? value)
    {
        if (_currentType is { } type && value is XmlSchema schema)
        {
            _types.Add(schema, type);
        }

        base.OnInsert(index, value);
    }
    #endregion
}