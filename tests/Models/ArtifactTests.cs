using Xunit;
using openrmf_msg_system.Models;
using System;

namespace tests.Models
{
    public class ArtifactTests
    {
        [Fact]
        public void Test_NewArtifactIsValid()
        {
            Artifact art = new Artifact();
            Assert.True(art != null);
        }
    
        [Fact]
        public void Test_ArtifactWithDataIsValid()
        {
            Artifact art = new Artifact();
            art.created = DateTime.Now;
            art.systemGroupId = "875678654gghjghjkgu658";
            art.hostName = "myHost";
            art.stigType = "Google Chrome";
            art.stigRelease = "R13";
            art.version = "1";
            art.rawChecklist = "<xml></xml>";
            art.updatedOn = DateTime.Now;

            // test things out
            Assert.True(art != null);
            Assert.True (!string.IsNullOrEmpty(art.created.ToShortDateString()));
            Assert.True (!string.IsNullOrEmpty(art.systemGroupId));
            Assert.True (!string.IsNullOrEmpty(art.hostName));
            Assert.True (!string.IsNullOrEmpty(art.stigType));
            Assert.True (!string.IsNullOrEmpty(art.stigRelease));
            Assert.True (!string.IsNullOrEmpty(art.rawChecklist));
            Assert.True (!string.IsNullOrEmpty(art.title));  // readonly from other fields
            Assert.True (art.updatedOn.HasValue);
            Assert.True (!string.IsNullOrEmpty(art.updatedOn.Value.ToShortDateString()));
        }
    }
}
