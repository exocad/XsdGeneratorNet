using System;

namespace XsdGenerator;

/// <summary>
/// The <see cref="ConsoleWriter"/> provides a method to write a message with a given <see cref="Priority"/>
/// to the console. The method can be used as <see cref="XsdGenerator.WriteLine"/> delegate, which is used
/// by the <see cref="XsdExporter"/> class.
/// </summary>
public static class ConsoleWriter
{
    /// <summary>
    /// Writes the given <paramref name="message"/> to the console and applies a foreground color depending
    /// on the <paramref name="priority"/>.
    /// </summary>
    /// <param name="priority">The message <see cref="Priority"/>.</param>
    /// <param name="message">The message to write.</param>
    public static void WriteLine(Priority priority, string message)
    {
        var color = Console.ForegroundColor;

        Console.ForegroundColor = priority switch
        {
            Priority.Error => ConsoleColor.Red,
            Priority.Warning => ConsoleColor.Yellow,
            _ => ConsoleColor.Gray,
        };

        Console.WriteLine(message);
        Console.ForegroundColor = color;
    }
}