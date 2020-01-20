// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_msg_system.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace openrmf_msg_system.Data {
    public interface ISystemGroupRepository
    {
        Task<IEnumerable<SystemGroup>> GetAllSystemGroups();
        Task<SystemGroup> GetSystemGroup(string id);
        Task<bool> RemoveSystemGroup(string id);

        // update just a single system document
        Task<bool> UpdateSystemGroup(string id, SystemGroup body);
    }
}