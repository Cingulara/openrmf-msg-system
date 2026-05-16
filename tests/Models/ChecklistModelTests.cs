using openrmf_msg_system.Models;
using Xunit;

namespace openrmf_msg_system_tests.Models;

public class ChecklistModelTests
{
    [Fact]
    public void Checklist_CreatesNestedDefaults()
    {
        var checklist = new CHECKLIST();

        Assert.NotNull(checklist.ASSET);
        Assert.NotNull(checklist.STIGS);
        Assert.NotNull(checklist.STIGS.iSTIG);
        Assert.NotNull(checklist.STIGS.iSTIG.STIG_INFO);
        Assert.NotNull(checklist.STIGS.iSTIG.VULN);
        Assert.Empty(checklist.STIGS.iSTIG.VULN);
    }

    [Fact]
    public void ModelGraph_AssignsAndRetainsData()
    {
        var checklist = new CHECKLIST
        {
            ASSET = new ASSET
            {
                HOST_NAME = "host-01",
                HOST_IP = "10.0.0.1",
                WEB_OR_DATABASE = "WEB",
                WEB_DB_SITE = "site-1",
                WEB_DB_INSTANCE = "instance-1"
            },
            STIGS = new STIGS
            {
                iSTIG = new iSTIG
                {
                    STIG_INFO = new STIG_INFO(),
                    VULN = new()
                    {
                        new VULN
                        {
                            STATUS = "Open",
                            FINDING_DETAILS = "detail",
                            COMMENTS = "comment",
                            SEVERITY_OVERRIDE = "medium",
                            SEVERITY_JUSTIFICATION = "justified",
                            STIG_DATA = new()
                            {
                                new STIG_DATA
                                {
                                    VULN_ATTRIBUTE = "Rule_ID",
                                    ATTRIBUTE_DATA = "SV-00001r1"
                                }
                            }
                        }
                    }
                }
            }
        };

        checklist.STIGS.iSTIG.STIG_INFO.SI_DATA.Add(
            new SI_DATA { SID_NAME = "version", SID_DATA = "1" });

        Assert.Equal("host-01", checklist.ASSET.HOST_NAME);
        Assert.Equal("10.0.0.1", checklist.ASSET.HOST_IP);
        Assert.Equal("WEB", checklist.ASSET.WEB_OR_DATABASE);
        Assert.Single(checklist.STIGS.iSTIG.STIG_INFO.SI_DATA);
        Assert.Single(checklist.STIGS.iSTIG.VULN);
        Assert.Equal("Open", checklist.STIGS.iSTIG.VULN[0].STATUS);
        Assert.Single(checklist.STIGS.iSTIG.VULN[0].STIG_DATA);
        Assert.Equal("Rule_ID", checklist.STIGS.iSTIG.VULN[0].STIG_DATA[0].VULN_ATTRIBUTE);
        Assert.NotEqual("Closed", checklist.STIGS.iSTIG.VULN[0].STATUS);
    }
}
