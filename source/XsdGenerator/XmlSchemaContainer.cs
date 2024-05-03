using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XsdGenerator;

/// <summary>
/// The <see cref="XmlSchemaContainer"/> class inherits from <see cref="XmlSchemas"/>
/// to map a generated <see cref="XmlSchema"/> to the current <see cref="Type"/>.
/// 
/// It also manages the creation of the <see cref="XmlReflectionImporter"/> and
/// <see cref="XmlSchemaExporter"/> and manages the type import and export.
/// 
/// When iterating over the generated <see cref="XmlSchema"/> instances, the corresponding
/// type that was used to create the schema can be obtained by calling 
/// <see cref="TryGetAssociatedType(XmlSchema, out Type)"/>.
/// </summary>
internal sealed class XmlSchemaContainer : XmlSchemas
{
    private readonly Dictionary<XmlSchema, Type> _types = [];
    private readonly XmlReflectionImporter _importer = new();
    private readonly XmlSchemaExporter _exporter;
    private Type? _currentType;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSchemaContainer"/> class.
    /// </summary>
    public XmlSchemaContainer()
    {
        _exporter = new XmlSchemaExporter(this);
    }

    /// <summary>
    /// Enumerates all generates schemas with their correspoding type and index.
    /// </summary>
    /// <returns>A lazy collection containing the schema, its type, if available, and a zero-based index.</returns>
    public IEnumerable<(XmlSchema schema, Type? type, int index)> EnumerateSchemas()
    {
        var count = Count;

        for (var index = 0; index < count; index++)
        {
            var schema = this[index];

            if (TryGetAssociatedType(schema, out var type))
            {
                yield return (schema, type, index);
            }
            else
            {
                yield return (schema, null, index);
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="Type"/> from which the given <paramref name="schema"/> has been created.
    /// </summary>
    /// <param name="schema">The schema to obtain the corresponding type for.</param>
    /// <param name="type">The associated type or <c>null</c>, if not type information is present.</param>
    /// <returns><c>true</c>, if the type could be obtained.</returns>
    public bool TryGetAssociatedType(XmlSchema schema, [MaybeNullWhen(false)] out Type type) =>
        _types.TryGetValue(schema, out type);

    /// <summary>
    /// Generates an <see cref="XmlSchema"/> for the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to create a schema for.</param>
    /// <remarks>The resulting schema is stored together with the <paramref name="type"/>.</remarks>
    public void GenerateSchema(Type type)
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