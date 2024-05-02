using System;

namespace XsdGenerator;

public static class ConsoleWriter
{
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