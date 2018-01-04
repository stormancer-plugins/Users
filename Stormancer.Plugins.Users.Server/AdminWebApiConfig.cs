using Stormancer.Server.AdminApi;
using System.Web.Http;

namespace Stormancer.Server.Users
{
    class AdminWebApiConfig : IAdminWebApiConfig
    {
        public void Configure(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("users.search", "_users/search", new { Controller = "UsersAdmin", Action="search"});
            config.Routes.MapHttpRoute("users", "_users/{id}", new { Controller = "UsersAdmin" });
        }
    }
}
