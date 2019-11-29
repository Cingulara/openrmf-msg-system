using openrmf_msg_system.Models;
using System.Collections.Generic;
using System;
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