# Xsd Generator for .NET Core

A tool to generate XSD files from .NET Core assemblies.

## Requirements

The [.NET 6 Desktop Runtime](https://download.visualstudio.microsoft.com/download/pr/d0849e66-227d-40f7-8f7b-c3f7dfe51f43/37f8a04ab7ff94db7f20d3c598dc4d74/windowsdesktop-runtime-6.0.29-win-x64.exe) must be installed on the local machine.
More details can be found here: [Download .NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Usage

`XsdGeneratorTool ASSEMBLYPATH [/type:TYPENAME[,TYPENAMEx]*] [/output:OUTPUTDIR]`

### Example

`XsdGeneratorTool MyDotNetCoreAssembly.dll /type:MyType`

