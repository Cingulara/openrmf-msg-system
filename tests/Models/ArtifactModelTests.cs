using System;
using MongoDB.Bson;
using openrmf_msg_system.Models;
using Xunit;

namespace openrmf_msg_system_tests.Models;

public class ArtifactModelTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaults()
    {
        var artifact = new Artifact();

        Assert.False(artifact.isWebDatabase);
        Assert.Equal(string.Empty, artifact.webDatabaseSite);
        Assert.Equal(string.Empty, artifact.webDatabaseInstance);
        Assert.Null(artifact.updatedOn);
        Assert.NotNull(artifact.InternalIdString);
    }

    [Fact]
    public void Title_BuildsFromFields_WithTrimming()
    {
        var artifact = new Artifact
        {
            hostName = "  host01  ",
            stigType = "  RHEL  ",
            stigRelease = "  R3  ",
            version = "2"
        };

        Assert.Equal("host01-RHEL-V2-R3", artifact.title);
        Assert.NotEqual("  host01  -  RHEL  -V2-  R3  ", artifact.title);
    }

    [Fact]
    public void Title_UsesUnknownWhenHostMissing()
    {
        var artifact = new Artifact
        {
            hostName = null,
            stigType = "",
            stigRelease = "   ",
            version = "1"
        };

        Assert.Equal("Unknown--V1-", artifact.title);
        Assert.DoesNotContain("null", artifact.title, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void InternalIdString_ReflectsInternalId()
    {
        var internalId = ObjectId.GenerateNewId();
        var artifact = new Artifact { InternalId = internalId };

        Assert.Equal(internalId.ToString(), artifact.InternalIdString);
        Assert.NotEqual(ObjectId.Empty.ToString(), artifact.InternalIdString);
    }

    [Fact]
    public void Properties_RoundTripDataCorrectly()
    {
        var created = DateTime.UtcNow;
        var updated = created.AddMinutes(5);
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();

        var artifact = new Artifact
        {
            created = created,
            systemGroupId = "system-1",
            systemTitle = "System One",
            hostName = "host",
            stigType = "Chrome",
            stigRelease = "R13",
            version = "1",
            rawChecklist = "<checklist />",
            CHECKLIST = new CHECKLIST(),
            tags = new() { "prod", "critical" },
            updatedOn = updated,
            createdBy = createdBy,
            updatedBy = updatedBy,
            isWebDatabase = true,
            webDatabaseSite = "site-a",
            webDatabaseInstance = "instance-a"
        };

        Assert.Equal(created, artifact.created);
        Assert.Equal("system-1", artifact.systemGroupId);
        Assert.Equal("System One", artifact.systemTitle);
        Assert.Equal("host-Chrome-V1-R13", artifact.title);
        Assert.Equal("<checklist />", artifact.rawChecklist);
        Assert.NotNull(artifact.CHECKLIST);
        Assert.Equal(2, artifact.tags.Count);
        Assert.Equal(updated, artifact.updatedOn);
        Assert.Equal(createdBy, artifact.createdBy);
        Assert.Equal(updatedBy, artifact.updatedBy);
        Assert.True(artifact.isWebDatabase);
        Assert.Equal("site-a", artifact.webDatabaseSite);
        Assert.Equal("instance-a", artifact.webDatabaseInstance);
        Assert.NotEqual(string.Empty, artifact.title);
    }
}
