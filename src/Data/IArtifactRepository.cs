// Copyright (c) Cingulara 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_msg_system.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace openrmf_msg_system.Data {
    public interface IArtifactRepository
    {        
        // get the checklist and all its metadata in a record from the DB
        Task<Artifact> GetArtifact(string id);

        // return checklist records for a given system
        Task<IEnumerable<Artifact>> GetSystemArtifacts(string system);

        Task<bool> UpdateArtifact(string id, Artifact body);
    }
}