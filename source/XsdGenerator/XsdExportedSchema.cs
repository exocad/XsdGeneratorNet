using System;
using System.Xml.Schema;

namespace XsdGenerator;

/// <summary>
/// Representation of an exported schema.
/// </summary>
/// <param name="Filename">The filename of the generated XSD file.</param>
/// <param name="Schema">The schema that has been exported.</param>
/// <param name="Type">The type from which the schema has been created or <c>null</c>, if not present.</param>
public sealed record XsdExportedSchema(string Filename, XmlSchema Schema, Type? Type);