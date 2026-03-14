using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Responses;

namespace BibXmr.Client.Daemon;

public partial class MoneroDaemonClient
{
    /// <summary>
    /// Queries the daemon's <c>get_info</c> endpoint to determine whether the RPC interface is restricted or unrestricted.
    /// </summary>
    /// <param name="client">The daemon client to probe.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The detected RPC access level.</returns>
    private static async Task<MoneroDaemonRpcAccess> DetectRpcAccessAsync(MoneroDaemonClient client, CancellationToken cancellationToken)
    {
        DaemonInformation info = await client.GetDaemonInformationAsync(cancellationToken).ConfigureAwait(false);
        return info.IsRestrictedRpc ? MoneroDaemonRpcAccess.Restricted : MoneroDaemonRpcAccess.Unrestricted;
    }
}
