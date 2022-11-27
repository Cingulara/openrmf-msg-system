// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_msg_system.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;

namespace openrmf_msg_system.Data
{
    public class ArtifactRepository : IArtifactRepository
    {
        private readonly ArtifactContext _context = null;

        public ArtifactRepository(Settings settings)
        {
            _context = new ArtifactContext(settings);
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        // query after Id or InternalId (BSonId value)
        //
        public async Task<Artifact> GetArtifact(string id)
        {
            return await _context.Artifacts.Find(artifact => artifact.InternalId == GetInternalId(id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Artifact>> GetSystemArtifacts(string systemGroupId)
        {
            var query = await _context.Artifacts.FindAsync(artifact => artifact.systemGroupId == systemGroupId);
            return query.ToList().OrderBy(x => x.title);
        }

        public async Task<bool> UpdateArtifact(string id, Artifact body)
        {
            var filter = Builders<Artifact>.Filter.Eq(s => s.InternalId, GetInternalId(id));
            body.InternalId = GetInternalId(id);
            var actionResult = await _context.Artifacts.ReplaceOneAsync(filter, body);
            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }
    }
}