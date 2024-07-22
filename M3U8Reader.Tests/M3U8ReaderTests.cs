using FluentAssertions;
using M3U8Reader.Tests.Infra;
using Xunit;

namespace M3U8Reader.Tests;

public class M3U8ReaderTests
{
    [Fact]
    public void CorrectlyIdentifiesTheLastResolution()
    {
        var stream = ResourceLoader.LoadFromEmbedded("manifest.m3u8");
        var reader = new M3U8Reader(stream!);
		
        var extensionValues = reader
            .Last(line => line is
            {
                LineKind: M3U8LineKind.Extension, 
                Extension.ExtensionName: M3U8Extensions.ExtStreamInf
            })
            .Extension.Values;
		
        var value = extensionValues[M3U8Extensions.ValueKeys.Resolution];
		
        value.ValueKind.Should().Be(M3U8ValueKind.Unquoted);
        value.Value.ToString().Should().Be("720x960");
    }

    [Fact]
    public void UseCase_SummingUpAllSegmentDurations()
    {
        var stream = ResourceLoader.LoadFromEmbedded("video.m3u8");
        var reader = new M3U8Reader(stream!);

        var sumOfExtInfos = reader
            .Where(line => line is {LineKind: M3U8LineKind.Extension, Extension.ExtensionName: M3U8Extensions.ExtInf})
            .Select(line => line.Extension.Values.Value.ToString())
            .Select(double.Parse)
            .Sum();
        sumOfExtInfos.Should().BeApproximately(54.5762, 0.0001);
    }

    [Fact]
    public void UseCase_ReadSourceAfterFindingRightExtInf()
    {
        var stream = ResourceLoader.LoadFromEmbedded("manifest.m3u8");
        var reader = new M3U8Reader(stream!);

        var extensionValues = reader
            .Where(l =>
                l.LineKind is M3U8LineKind.Extension && l.Extension.ExtensionName == M3U8Extensions.ExtStreamInf ||
                l.LineKind is M3U8LineKind.Source)
            .ToList();
        var index = extensionValues.FindLastIndex(l => l is
            { LineKind: M3U8LineKind.Extension, Extension.ExtensionName: M3U8Extensions.ExtStreamInf });
        var source = extensionValues[index + 1].Value;
        source.Should().Be("video_3400000.m3u8(encryption=cbc)");

    }
}