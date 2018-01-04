using Stormancer.Server.API;
using Stormancer.Diagnostics;
using Stormancer.Platform.Core.Cryptography;
using Stormancer.Plugins;
using Stormancer.Server.Components;
using System.Threading.Tasks;

namespace Stormancer.Server.Users
{
    internal class SceneAuthorizationController : ControllerBase
    {
        private readonly Management.ManagementClientAccessor _accessor;
        private readonly UserManagementConfig _config;
        private readonly IUserSessions _sessions;
        private readonly ILogger _logger;
        private readonly IEnvironment _environment;

        public SceneAuthorizationController(Management.ManagementClientAccessor accessor, IEnvironment environment, UserManagementConfig config, IUserSessions sessions, ILogger logger)
        {
            _logger = logger;
            _accessor = accessor;
            _config = config;
            _sessions = sessions;
            _environment = environment;
        }
        public async Task GetToken(RequestContext<IScenePeerClient> ctx)
        {
            _logger.Log(LogLevel.Trace, "authorization", "Receiving a token request to access a scene", new { });

            var client = await _accessor.GetApplicationClient();

            var user = await _sessions.GetUser(ctx.RemotePeer);
            if (user == null)
            {
                throw new ClientException("Client is not logged in.");
            }
            var sceneId = ctx.ReadObject<string>();
            _logger.Log(LogLevel.Debug, "authorization", $"Authorizing access to scene '{sceneId}'", new { sceneId, user.Id });
            var token = await client.CreateConnectionToken(sceneId, new byte[0], "application/octet-stream");

            ctx.SendValue(token);
        }

        public async Task GetBearerToken(RequestContext<IScenePeerClient> ctx)
        {
            var app = await _environment.GetApplicationInfos();
            var session = await _sessions.GetSession(ctx.RemotePeer);
            ctx.SendValue(TokenGenerator.CreateToken(new BearerTokenData { UserId = session.User.Id }, app.PrimaryKey));
        }

        public class BearerTokenData
        {
            public string UserId { get; set; }
        }
        public async Task GetUserFromBearerToken(RequestContext<IScenePeerClient> ctx)
        {
            var app = await _environment.GetApplicationInfos();
            var data = TokenGenerator.DecodeToken<BearerTokenData>(ctx.ReadObject<string>(), app.PrimaryKey);
            if (data == null)
            {
                throw new ClientException("Invalid Token");
            }
            ctx.SendValue(data.UserId);
        }
    }
}