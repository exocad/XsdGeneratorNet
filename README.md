# Xsd Generator for .NET Core

A tool to generate XSD files from types defined in .NET Core assemblies. The [Microsoft XSD Tool](https://learn.microsoft.com/en-us/dotnet/standard/serialization/xml-schema-definition-tool-xsd-exe)
offers the same functionality for .NET Framework assemblies only, which is why this tool has been written.

## License

MIT

## Requirements

The [.NET 6 Desktop Runtime](https://download.visualstudio.microsoft.com/download/pr/d0849e66-227d-40f7-8f7b-c3f7dfe51f43/37f8a04ab7ff94db7f20d3c598dc4d74/windowsdesktop-runtime-6.0.29-win-x64.exe) must be installed on the local machine.
Alternatively, the Runtime can be installed with winget by opening a PowerShell and executing:

```bash
winget install Microsoft.DotNet.DesktopRuntime.6
```

More details can be found here: [Download .NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Usage

The command line arguments are compatible with the original tool with some additional options.

`XsdGeneratorTool ASSEMBLYPATH[ ASSEMBLYPATHx]** [/type:TYPENAME[,TYPENAMEx]*] [/output:OUTPUTDIR] [/skip-validation] [/help]`

| Command | Description | Example |
| :---    | :---        | :---    |
| `ASSEMBLYPATH`  | One or more assemblies with types to export. Paths with spaces must be surrounded with a `"` character. | `MyAssembly.dll`
| `/type:TYPENAME[,TYPENAME]**` | If omitted, all suitable types will be exported. Otherwise, types matching the given names will be exported. When a typename ends with `.*`, all types whose full name starts with the given string will be included. | `/type:MyType,MyNamespace.*` |
| `/output:OUTPUTDIR` | Specifies the directory to copy the generates XSD files to. | `/output:C:\Temp` |
| `/skip-validation` | If specified, XML schema validation warnings will be ignored and not logged. Errors will still abort the export operation, though. | |
| `/help` | Prints a usage hint | |

### Example

`XsdGeneratorTool MyDotNetCoreAssembly.dll /type:MyType /output:C:\MyXsdFiles`

## References

- .NET 4.8 reference source code of the [XSD tool](https://github.com/microsoft/referencesource/blob/master/xsd/microsoft/devapps/xsd/xsd.cs).
- Microsoft XML Schema Definition Tool [Documentation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/xml-schema-definition-tool-xsd-exe).
