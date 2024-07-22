# M3U8 Reader

![dotnet build & test](https://github.com/flq/M3U8Reader/actions/workflows/dotnet.yml/badge.svg)

This NuGet helps you in reading M3U8 files and gives you access to each line of the file.

It will tell you whether the line is an extension or a source that is being pointed at.

You can install the [NuGet package](https://www.nuget.org/packages/M3U8Reader) by running the following command:

```bash
dotnet add package M3U8Reader
```

## Example

Here is an example usage to sum up all duration infos of referenced segments:

```csharp
var reader = new M3U8Reader(stream);

var sumOfExtInfos = reader
    .Where(line => line is {LineKind: M3U8LineKind.Extension, Extension.ExtensionName: M3U8Extensions.ExtInf})
    .Select(line => line.Extension.Values.Value.ToString())
    .Select(double.Parse)
    .Sum();
```

This one shows how to read the duration value associated with a specific streaming info:

```csharp
var reader = new M3U8Reader(stream!);
		
var extensionValues = reader
    .Last(line => line is
    {
        LineKind: M3U8LineKind.Extension, 
        Extension.ExtensionName: M3U8Extensions.ExtStreamInf
    })
    .Extension.Values;

var value = extensionValues[M3U8Extensions.ValueKeys.Resolution];
```