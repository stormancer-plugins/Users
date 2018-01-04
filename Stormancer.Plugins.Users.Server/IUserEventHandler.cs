using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace Stormancer.Server.Users
{
    public interface IUserEventHandler
    {

        Task OnMergingUsers(IEnumerable<Server.Users.User> users);

        Task<Object> OnMergedUsers(IEnumerable<Server.Users.User> enumerable, Server.Users.User mainUser);
        BulkDescriptor OnBuildMergeQuery(IEnumerable<Server.Users.User> enumerable, Server.Users.User mainUser, object data, BulkDescriptor desc);
    }
}
