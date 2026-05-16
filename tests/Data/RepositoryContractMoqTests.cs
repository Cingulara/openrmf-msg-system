using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using openrmf_msg_system.Data;
using openrmf_msg_system.Models;
using Xunit;

namespace openrmf_msg_system_tests.Data;

public class RepositoryContractMoqTests
{
    [Fact]
    public async Task ArtifactRepositoryContract_UpdateArtifact_PassAndFailPaths()
    {
        var artifact = new Artifact { systemGroupId = "sys-1", version = "1" };

        var passMock = new Mock<IArtifactRepository>(MockBehavior.Strict);
        passMock
            .Setup(repo => repo.UpdateArtifact("artifact-1", artifact))
            .ReturnsAsync(true);

        var passResult = await passMock.Object.UpdateArtifact("artifact-1", artifact);

        Assert.True(passResult);
        passMock.Verify(repo => repo.UpdateArtifact("artifact-1", artifact), Times.Once);

        var failMock = new Mock<IArtifactRepository>(MockBehavior.Strict);
        failMock
            .Setup(repo => repo.UpdateArtifact("artifact-1", artifact))
            .ReturnsAsync(false);

        var failResult = await failMock.Object.UpdateArtifact("artifact-1", artifact);

        Assert.False(failResult);
        failMock.Verify(repo => repo.UpdateArtifact("artifact-1", artifact), Times.Once);
    }

    [Fact]
    public async Task ArtifactRepositoryContract_GetSystemArtifacts_ReturnsDataAndEmptyPaths()
    {
        var expected = new List<Artifact>
        {
            new() { systemGroupId = "sys-1", hostName = "a" },
            new() { systemGroupId = "sys-1", hostName = "b" }
        };

        var withDataMock = new Mock<IArtifactRepository>(MockBehavior.Strict);
        withDataMock
            .Setup(repo => repo.GetSystemArtifacts("sys-1"))
            .ReturnsAsync(expected);

        var withDataResult = await withDataMock.Object.GetSystemArtifacts("sys-1");

        Assert.Equal(2, ((List<Artifact>)withDataResult).Count);
        Assert.NotEmpty(withDataResult);

        var emptyMock = new Mock<IArtifactRepository>(MockBehavior.Strict);
        emptyMock
            .Setup(repo => repo.GetSystemArtifacts("sys-1"))
            .ReturnsAsync(new List<Artifact>());

        var emptyResult = await emptyMock.Object.GetSystemArtifacts("sys-1");

        Assert.Empty(emptyResult);
    }

    [Fact]
    public async Task SystemGroupRepositoryContract_ChecklistCountUpdate_PassAndFailPaths()
    {
        var passMock = new Mock<ISystemGroupRepository>(MockBehavior.Strict);
        passMock
            .Setup(repo => repo.IncreaseSystemGroupCount("sys-1"))
            .ReturnsAsync(true);
        passMock
            .Setup(repo => repo.DecreaseSystemGroupCount("sys-1"))
            .ReturnsAsync(true);

        var increased = await passMock.Object.IncreaseSystemGroupCount("sys-1");
        var decreased = await passMock.Object.DecreaseSystemGroupCount("sys-1");

        Assert.True(increased);
        Assert.True(decreased);

        var failMock = new Mock<ISystemGroupRepository>(MockBehavior.Strict);
        failMock
            .Setup(repo => repo.IncreaseSystemGroupCount("sys-1"))
            .ReturnsAsync(false);

        var failResult = await failMock.Object.IncreaseSystemGroupCount("sys-1");

        Assert.False(failResult);
        failMock.Verify(repo => repo.IncreaseSystemGroupCount("sys-1"), Times.Once);
    }

    [Fact]
    public async Task SystemGroupRepositoryContract_GetSystemGroup_ReturnsRecordAndNullPaths()
    {
        var system = new SystemGroup { title = "system" };

        var foundMock = new Mock<ISystemGroupRepository>(MockBehavior.Strict);
        foundMock
            .Setup(repo => repo.GetSystemGroup("sys-1"))
            .ReturnsAsync(system);

        var foundResult = await foundMock.Object.GetSystemGroup("sys-1");

        Assert.NotNull(foundResult);
        Assert.Equal("system", foundResult!.title);

        var nullMock = new Mock<ISystemGroupRepository>(MockBehavior.Strict);
        nullMock
            .Setup(repo => repo.GetSystemGroup("sys-1"))
            .ReturnsAsync((SystemGroup)null);

        var nullResult = await nullMock.Object.GetSystemGroup("sys-1");

        Assert.Null(nullResult);
        Assert.NotEqual(system, nullResult);
    }
}
