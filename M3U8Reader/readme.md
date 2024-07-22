# M3U8 Reader

This NuGet helps you in reading M3U8 files and gives you access to each line of the file.

It will tell you whether the line is an extension or a source that is being pointed at.

You can install the NuGet package by running the following command:

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