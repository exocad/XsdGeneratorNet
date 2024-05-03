using System;
using System.Collections.Generic;
using System.Reflection;

namespace XsdGenerator;

/// <summary>
/// The <see cref="TypeList"/> class is responsible for loading an assembly and locating the types to export.
/// </summary>
internal sealed class TypeList
{
    private readonly WriteLine _writeLine;
    private readonly StringComparison _stringComparison;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeList"/> class.
    /// </summary>
    /// <param name="assemblyPath">
    /// The path to the assembly to load and to export types from.
    /// </param>
    /// <param name="typeNames">
    /// A list of typenames to export. Appending a <c>.*</c> to a typename will include all types
    /// whose full name starts with the given string.
    /// </param>
    /// <param name="config">
    /// The exporter configuration.
    /// </param>
    public TypeList(string assemblyPath, IReadOnlyList<string> typeNames, XsdExporterConfig config)
    {
        _writeLine = config.WriteLine;
        _stringComparison = config.TypeNameStringComparison;

        if (LoadAssembly(assemblyPath) is not { } assembly)
        {
            return;
        }

        var result = new List<Type>();

        foreach (var type in assembly.GetTypes())
        {
            var skipType =
                type.IsPublic is false ||
                type is { IsAbstract: true, IsSealed: true } ||
                type.IsInterface ||
                type.ContainsGenericParameters;

            if (skipType)
            {
                continue;
            }

            InspectType(type, typeNames, result);
        }

        Assembly = assembly;
        ExportableTypes = result;
    }

    /// <summary>
    /// Gets the corresponding assembly or <c>null</c>, if it could not be loaded.
    /// </summary>
    public Assembly? Assembly { get; }

    /// <summary>
    /// Gets a list with the types to export.
    /// </summary>
    public IReadOnlyList<Type> ExportableTypes { get; } = [];

    private void WriteLine(Priority priority, string message)
    {
        _writeLine(priority, message);
    }

    private Assembly? LoadAssembly(string assemblyPath)
    {
        try
        {
            return Assembly.LoadFrom(assemblyPath);
        }
        catch (Exception ex)
        {
            WriteLine(Priority.Error, $"Failed to load assembly '{assemblyPath}': {ex.Message}");

            return null;
        }
    }

    private void InspectType(Type type, IReadOnlyList<string> typeNames, IList<Type> exportableTypes)
    {
        if (type.FullName is null)
        {
            return;
        }

        if (typeNames.Count == 0)
        {
            exportableTypes.Add(type);
            WriteLine(Priority.Normal, $"Adding '{type.FullName}' to the list of exportable types.");
            return;
        }

        foreach (var typeName in typeNames)
        {
            var isExportableType =
                typeName.Equals(type.FullName, _stringComparison) ||
                typeName.Equals(type.Name, _stringComparison) ||
                (typeName.EndsWith(".*") && type.FullName.StartsWith(typeName[..^2], _stringComparison));

            if (isExportableType)
            {
                exportableTypes.Add(type);
                WriteLine(Priority.Normal, $"Adding '{type.FullName}' to the list of exportable types.");
                return;
            }
        }
    }
}