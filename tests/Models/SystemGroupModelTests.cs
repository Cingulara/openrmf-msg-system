using System;
using MongoDB.Bson;
using openrmf_msg_system.Models;
using Xunit;

namespace openrmf_msg_system_tests.Models;

public class SystemGroupModelTests
{
    [Fact]
    public void Constructor_DefaultsAreExpected()
    {
        var systemGroup = new SystemGroup();

        Assert.Equal(0, systemGroup.numberOfChecklists);
        Assert.Null(systemGroup.updatedOn);
        Assert.Null(systemGroup.lastComplianceCheck);
        Assert.Equal(ObjectId.Empty.ToString(), systemGroup.InternalIdString);
    }

    [Fact]
    public void Properties_RoundTripDataCorrectly()
    {
        var created = DateTime.UtcNow;
        var updated = created.AddHours(2);
        var compliance = created.AddHours(1);
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();

        var systemGroup = new SystemGroup
        {
            InternalId = ObjectId.GenerateNewId(),
            created = created,
            title = "Core Infra",
            description = "Core infrastructure package",
            numberOfChecklists = 4,
            rawNessusFile = "<nessus />",
            nessusFilename = "infra.nessus",
            updatedOn = updated,
            lastComplianceCheck = compliance,
            createdBy = createdBy,
            updatedBy = updatedBy
        };

        Assert.Equal("Core Infra", systemGroup.title);
        Assert.Equal("Core infrastructure package", systemGroup.description);
        Assert.Equal(4, systemGroup.numberOfChecklists);
        Assert.Equal("<nessus />", systemGroup.rawNessusFile);
        Assert.Equal("infra.nessus", systemGroup.nessusFilename);
        Assert.Equal(created, systemGroup.created);
        Assert.Equal(updated, systemGroup.updatedOn);
        Assert.Equal(compliance, systemGroup.lastComplianceCheck);
        Assert.Equal(createdBy, systemGroup.createdBy);
        Assert.Equal(updatedBy, systemGroup.updatedBy);
        Assert.NotEqual(ObjectId.Empty.ToString(), systemGroup.InternalIdString);
    }

    [Fact]
    public void ChecklistCount_CanRepresentDecreaseWithoutUnexpectedMutation()
    {
        var systemGroup = new SystemGroup
        {
            numberOfChecklists = 1
        };

        systemGroup.numberOfChecklists -= 1;

        Assert.Equal(0, systemGroup.numberOfChecklists);
        Assert.NotEqual(1, systemGroup.numberOfChecklists);
    }
}
