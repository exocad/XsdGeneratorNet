using System;
using System.IO;
using XsdGenerator;

namespace XsdGeneratorTool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(string.Join(" ", args));
            PrintHeader();

            try
            {
                var commandLine = CommandLine.Parse(args, ConsoleWriter.WriteLine);

                if (commandLine.ShowHelp)
                {
                    PrintHelp();
                    return;
                }

                PrintCommandLineConfig(commandLine);

                var exporter = new XsdExporter(XsdExporterConfig.Default with
                {
                    SkipSchemaValidation = commandLine.SkipSchemaValidation,
                    OutputDirectory = commandLine.OutputDirectory,
                });

                var result = exporter.ExportSchemas(commandLine.AssemblyPaths, commandLine.TypeNames);

                if (result.HasValidationWarnings)
                {
                    var color = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("The XML schema validation completed with warnings.");
                    Console.ForegroundColor = color;
                }

                Console.WriteLine();
                Console.WriteLine("The following files were written:");

                foreach (var exportedSchema in result.ExportedSchemas)
                {
                    if (exportedSchema.Type is { } type)
                    {
                        Console.WriteLine($"- {exportedSchema.Filename} for type {type.FullName}");
                    }
                    else
                    {
                        Console.WriteLine($"- {exportedSchema.Filename}");
                    }
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
                PrintHelp();
            }
        }

        private static void PrintException(Exception ex)
        {
            var color = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex);
            Console.WriteLine();
            Console.ForegroundColor = color;
        }

        private static void PrintHeader()
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(new string('=', 80));
            Console.WriteLine("XSD Generator Tool for .NET Core");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine();
        }

        private static void PrintCommandLineConfig(CommandLine commandLine)
        {
            Console.WriteLine($"OutputDir:          {commandLine.OutputDirectory}");
            Console.WriteLine($"Skip-Validation:    {commandLine.SkipSchemaValidation}");
            Console.WriteLine($"AssemblyNames:      {string.Join("\r\n                    ", commandLine.AssemblyPaths)}");
            Console.WriteLine($"TypeNames:          {string.Join("\r\n                    ", commandLine.TypeNames)}");
            Console.WriteLine();
        }

        private static void PrintHelp()
        {
            var filename = Path.GetFileNameWithoutExtension(Environment.ProcessPath)
                ?? nameof(XsdGeneratorTool);

            Console.WriteLine("Usage:");
            Console.WriteLine($"{filename} AssemblyFilename [AssemblyFilename*] [/type:TYPE[,TYPE2]*] [/output:OUTPUTDIR] [/skip-validation] [/help]");
        }
    }
}
