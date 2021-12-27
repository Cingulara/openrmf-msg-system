using MongoDB.Driver;
using openrmf_msg_system.Models;

namespace openrmf_msg_system.Data
{
    public class ArtifactContext
    {
        private readonly IMongoDatabase _database = null;

        public ArtifactContext(Settings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Database);
        }

        public IMongoCollection<Artifact> Artifacts
        {
            get
            {
                return _database.GetCollection<Artifact>("Artifacts");
            }
        }

        public IMongoCollection<SystemGroup> SystemGroups
        {
            get
            {
                return _database.GetCollection<SystemGroup>("SystemGroups");
            }
        }
    }
}