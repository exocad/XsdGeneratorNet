using System;
using System.Collections.Generic;
using XsdGenerator;

namespace XsdGeneratorTool;

/// <summary>
/// The <see cref="CommandLine"/> class provides the options that were passed on application start.
/// </summary>
internal sealed class CommandLine
{
    /// <summary>
    /// Parses the command line arguments and creates an instance of the <see cref="CommandLine"/>
    /// class from it.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <param name="writeLine">The callback to use when writing log messages.</param>
    /// <returns>A <see cref="CommandLine"/> instance that was created from the command line arguments.</returns>
    public static CommandLine Parse(string[] args, WriteLine writeLine)
    {
        var assemblies = new List<string>();
        var typeNames = new List<string>();
        var count = args.Length;
        var showHelp = false;
        var skipSchemaValidation = false;
        var outputDirectory = default(string?);

        for (var index = 0; index < count; index++)
        {
            if (TryReadTypeName(args, index, typeNames, writeLine) ||
                TryReadAssemblyName(args, index, assemblies, writeLine))
            {
                continue;
            }

            if (outputDirectory is null && TryReadOutputDirectory(args, index, writeLine, out var directory))
            {
                outputDirectory = directory;
            }

            showHelp |= IsHelpArg(args[index]);
            skipSchemaValidation |= IsSkipValidationArg(args[index]);
        }

        showHelp |= assemblies.Count == 0;

        return new CommandLine(assemblies, typeNames)
        {
            ShowHelp = showHelp,
            SkipSchemaValidation = skipSchemaValidation,
            OutputDirectory = outputDirectory ?? Environment.CurrentDirectory,
        };
    }

    /// <summary>
    /// Gets a value indicating whether the help should be displayed. If set, no other
    /// operations will be executed.
    /// </summary>
    public bool ShowHelp { get; init; }

    /// <summary>
    /// Gets a value indicating whether the XML schema validation shall be skipped.
    /// </summary>
    public bool SkipSchemaValidation { get; init; }

    /// <summary>
    /// Gets the output directory for the generated XSD files.
    /// </summary>
    public string OutputDirectory { get; init; } = null!;

    /// <summary>
    /// Gets the paths of the .NET assemblies to load and to export types from.
    /// </summary>
    public IReadOnlyList<string> AssemblyPaths { get; }

    /// <summary>
    /// Gets the type names to export.
    /// </summary>
    public IReadOnlyList<string> TypeNames { get; }

    private CommandLine(IReadOnlyList<string> assemblyPaths, IReadOnlyList<string> typeNames) => (AssemblyPaths, TypeNames) = (assemblyPaths, typeNames);

    private static bool IsHelpArg(string arg)
    {
        return arg.Equals("/help", StringComparison.InvariantCultureIgnoreCase)
            || arg.Equals("--help", StringComparison.InvariantCultureIgnoreCase)
            || arg.Equals("-h", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsSkipValidationArg(string arg)
    {
        return arg.Equals("/skip-validation", StringComparison.InvariantCultureIgnoreCase)
            || arg.Equals("--skip-validation", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool TryGetOptionValue(string[] names, string[] args, int index, WriteLine writeLine, out string value)
    {
        var arg = args[index];

        foreach (var name in names)
        {
            if (arg.StartsWith(name, StringComparison.InvariantCultureIgnoreCase) &&
                arg.Length > name.Length && arg[name.Length] == ':')
            {
                value = arg[(name.Length + 1)..];
                return true;
            }

            if (arg.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (index + 1 >= arg.Length)
                {
                    writeLine(Priority.Warning, $"Missing value after {arg}.");
                    value = string.Empty;
                    return false;
                }

                value = args[1 + index];
                return true;
            }
        }

        value = string.Empty;
        return false;
    }

    private static bool TryReadTypeName(string[] args, int index, List<string> typeNames, WriteLine writeLine)
    {
        if (TryGetOptionValue(["/type", "--type", "-t", "/t"], args, index, writeLine, out var value))
        {
            AppendTypeNames(value, typeNames);
            return true;
        }

        return false;

        static void AppendTypeNames(string typeNameString, List<string> typeNames)
        {
            typeNames.AddRange(typeNameString.Split(new [] {',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }
    }

    private static bool TryReadOutputDirectory(string[] args, int index, WriteLine writeLine, out string directory)
    {
        return TryGetOptionValue(["/output", "--output", "-o", "/o"], args, index, writeLine, out directory);
    }

    private static bool TryReadAssemblyName(string[] args, int index, List<string> assemblyNames, WriteLine writeLine)
    {
        var arg = args[index].Trim('"');

        if (arg.StartsWith('-') || arg.StartsWith('/'))
        {
            return false;
        }

        if (arg.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) || arg.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
        {
            assemblyNames.Add(arg);
            return true;
        }

        return false;
    }
}