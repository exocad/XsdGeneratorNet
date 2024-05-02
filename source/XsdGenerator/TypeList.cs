using System;
using System.Collections.Generic;
using System.Reflection;

namespace XsdGenerator;

internal sealed class TypeList
{
    private readonly WriteLine _writeLine;
    private readonly StringComparison _stringComparison;

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

    public Assembly? Assembly { get; }

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