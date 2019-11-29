using MongoDB.Driver;
using openrmf_msg_system.Models;
using Microsoft.Extensions.Options;

namespace openrmf_msg_system.Data
{
    public class SystemGroupContext
    {
        private readonly IMongoDatabase _database = null;

        public SystemGroupContext(Settings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Database);
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