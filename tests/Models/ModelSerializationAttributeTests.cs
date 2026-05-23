using System.Reflection;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using openrmf_msg_system.Models;
using Xunit;

namespace openrmf_msg_system_tests.Models;

public class ModelSerializationAttributeTests
{
    [Fact]
    public void XmlElementAttributes_ArePresentOnExpectedProperties()
    {
        var stigInfoProp = typeof(STIG_INFO).GetProperty(nameof(STIG_INFO.SI_DATA));
        var vulnProp = typeof(VULN).GetProperty(nameof(VULN.STIG_DATA));
        var iStigProp = typeof(iSTIG).GetProperty(nameof(iSTIG.VULN));

        var stigInfoAttr = stigInfoProp?.GetCustomAttribute<XmlElementAttribute>();
        var vulnAttr = vulnProp?.GetCustomAttribute<XmlElementAttribute>();
        var iStigAttr = iStigProp?.GetCustomAttribute<XmlElementAttribute>();

        Assert.NotNull(stigInfoAttr);
        Assert.NotNull(vulnAttr);
        Assert.NotNull(iStigAttr);
        Assert.Equal("SI_DATA", stigInfoAttr!.ElementName);
        Assert.Equal("STIG_DATA", vulnAttr!.ElementName);
        Assert.Equal("VULN", iStigAttr!.ElementName);
        Assert.NotEqual("WRONG", iStigAttr.ElementName);
    }

    [Fact]
    public void BsonAttributes_ArePresentOnExpectedProperties()
    {
        var artifactIdProp = typeof(Artifact).GetProperty(nameof(Artifact.InternalId));
        var artifactUpdatedProp = typeof(Artifact).GetProperty(nameof(Artifact.updatedOn));
        var systemIdProp = typeof(SystemGroup).GetProperty(nameof(SystemGroup.InternalId));
        var systemCreatedProp = typeof(SystemGroup).GetProperty(nameof(SystemGroup.created));

        Assert.NotNull(artifactIdProp?.GetCustomAttribute<BsonIdAttribute>());
        Assert.NotNull(artifactUpdatedProp?.GetCustomAttribute<BsonDateTimeOptionsAttribute>());
        Assert.NotNull(systemIdProp?.GetCustomAttribute<BsonIdAttribute>());
        Assert.NotNull(systemCreatedProp?.GetCustomAttribute<BsonDateTimeOptionsAttribute>());
        Assert.Null(typeof(Settings).GetProperty(nameof(Settings.ConnectionString))?.GetCustomAttribute<BsonIdAttribute>());
    }
}
