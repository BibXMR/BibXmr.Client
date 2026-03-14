using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Network;
using BibXmr.Client.Wallet.Dto.Requests;
using BibXmr.Client.Wallet.Dto.Responses;

namespace BibXmr.Client.Network.Serialization
{
    /// <summary>
    /// Declares shared root DTO types for JSON source generation.
    /// </summary>
    internal sealed class CommonJsonTypeRoots
    {
        /// <summary>
        /// Gets or sets the anonymous request.
        /// </summary>
        public AnonymousRequest? AnonymousRequest { get; set; }

        /// <summary>
        /// Gets or sets the generic request parameters.
        /// </summary>
        public GenericRequestParameters? GenericRequestParameters { get; set; }

        /// <summary>
        /// Gets or sets the custom request parameters.
        /// </summary>
        public CustomRequestParameters? CustomRequestParameters { get; set; }

        /// <summary>
        /// Gets or sets the rpc response.
        /// </summary>
        public RpcResponse? RpcResponse { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public Error? Error { get; set; }

        /// <summary>
        /// Gets or sets the rpc result response.
        /// </summary>
        public RpcResultResponse<object>? RpcResultResponse { get; set; }

        /// <summary>
        /// Gets or sets the address index parameter.
        /// </summary>
        public AddressIndexParameter? AddressIndexParameter { get; set; }

        /// <summary>
        /// Gets or sets the daemon output request.
        /// </summary>
        public DaemonOutputRequest? DaemonOutputRequest { get; set; }

        /// <summary>
        /// Gets or sets the height array parameters.
        /// </summary>
        public ulong[]? HeightArrayParameters { get; set; }
    }
}
