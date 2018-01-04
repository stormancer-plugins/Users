using System.Threading.Tasks;

namespace Stormancer.Server.Users
{
    public interface IUserSessionEventHandler
    {
        Task OnLoggedIn(IScenePeerClient client, User user, PlatformId platformId);

        Task OnLoggedOut(long peerId, User user);
    }
}
