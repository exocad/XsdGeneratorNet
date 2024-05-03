namespace XsdGenerator;

/// <summary>
/// A delegate that is used by the <see cref="XsdExporter"/> when writing log messages.
/// </summary>
/// <param name="priority">The message priority.</param>
/// <param name="message">The message to write.</param>
public delegate void WriteLine(Priority priority, string message);
