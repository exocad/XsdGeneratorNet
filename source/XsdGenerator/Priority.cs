namespace XsdGenerator;

/// <summary>
/// An enumeration containing the supported priority levels for log messages.
/// </summary>
public enum Priority
{
    /// <summary>
    /// Standard priority for informational messages.
    /// </summary>
    Normal,

    /// <summary>
    /// A warning indicates a potential issue which does not affect schema generation,
    /// but it may lead to unexpected behavior when using the generated file.
    /// </summary>
    Warning,

    /// <summary>
    /// An error indicates an issue with an exported C# type, the assembly to load or the
    /// XSD file to write. The export operation cannot be completed.
    /// </summary>
    Error,
}