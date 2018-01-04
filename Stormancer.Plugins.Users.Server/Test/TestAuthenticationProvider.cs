using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stormancer.Core;
using Newtonsoft.Json.Linq;

namespace Stormancer.Server.Users.Test
{
    public class TestAuthenticationProvider : IAuthenticationProvider
    {
        public const string PROVIDER_NAME = "test";

        public void AddMetadata(Dictionary<string, string> result)
        {
            result.Add("provider.test", "enabled");
        }

        public async Task<AuthenticationResult> Authenticate(Dictionary<string, string> authenticationCtx, IUserService userService)
        {
            if (authenticationCtx["provider"] != PROVIDER_NAME)
            {
                return null;
            }

            string username;
            authenticationCtx.TryGetValue("pseudo", out username);

            var pId = new PlatformId { Platform = PROVIDER_NAME };
            if (string.IsNullOrWhiteSpace(username))
            {
                return AuthenticationResult.CreateFailure("Pseudo must not be empty", pId, authenticationCtx);
            }

            var user = await userService.GetUserByClaim(PROVIDER_NAME, "pseudo", username);

            if (user == null)
            {
                var uid = Guid.NewGuid().ToString("N");

                user = await userService.CreateUser(uid, JObject.FromObject(new { pseudo = username }));
                var claim = new JObject();
                claim["pseudo"] = username;
                user = await userService.AddAuthentication(user, PROVIDER_NAME, claim, username);
            }

            return AuthenticationResult.CreateSuccess(user, pId, authenticationCtx);
        }

        public void Initialize(ISceneHost scene)
        {
        }
    }
}
