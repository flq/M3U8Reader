using System.Reflection;

namespace M3U8Reader.Tests.Infra;

public static class ResourceLoader
{
    /// <summary>
    ///     Load a resource from the "TestResources" folder, in which you can drop embedded resources
    /// </summary>
    /// <param name="name">the filename (with extension)</param>
    public static Stream? LoadFromEmbedded(string name)
        => typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream($"M3U8Reader.Tests.TestResources.{name}");
    
}