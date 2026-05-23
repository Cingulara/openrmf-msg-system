using System;
using openrmf_msg_system.Classes;
using Xunit;

namespace openrmf_msg_system_tests.Classes;

public class CompressionTests
{
    [Fact]
    public void CompressThenDecompress_RoundTripsText()
    {
        const string text = "OpenRMF checklist payload";

        var compressed = Compression.CompressString(text);
        var decompressed = Compression.DecompressString(compressed);

        Assert.Equal(text, decompressed);
        Assert.NotEqual(text, compressed);
    }

    [Fact]
    public void CompressThenDecompress_RoundTripsEmptyString()
    {
        const string text = "";

        var compressed = Compression.CompressString(text);
        var decompressed = Compression.DecompressString(compressed);

        Assert.Equal(text, decompressed);
        Assert.NotNull(compressed);
    }

    [Fact]
    public void DecompressString_WithInvalidBase64_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Compression.DecompressString("not-base64"));
    }
}
