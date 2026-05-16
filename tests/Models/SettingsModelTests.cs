using openrmf_msg_system.Models;
using Xunit;

namespace openrmf_msg_system_tests.Models;

public class SettingsModelTests
{
    [Fact]
    public void NewSettings_StartWithNullFields()
    {
        var settings = new Settings();

        Assert.Null(settings.ConnectionString);
        Assert.Null(settings.Database);
    }

    [Fact]
    public void Fields_AssignAndRetainValues()
    {
        var settings = new Settings
        {
            ConnectionString = "mongodb://localhost:27017",
            Database = "openrmf"
        };

        Assert.Equal("mongodb://localhost:27017", settings.ConnectionString);
        Assert.Equal("openrmf", settings.Database);
        Assert.NotEqual("", settings.ConnectionString);
        Assert.NotEqual("", settings.Database);
    }
}
