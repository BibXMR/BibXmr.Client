namespace BibXmr.Client.Network
{
    /// <summary>
    /// Represents a JSON-RPC request envelope built by the network adapter.
    /// </summary>
    internal class AnonymousRequest
    {
        /// <summary>
        /// Gets or sets the jsonrpc version.
        /// </summary>
        public string? Jsonrpc { get; set; } = FieldAndHeaderDefaults.JsonRpc;

        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// Gets the get.
        /// </summary>
        public object? Params { get; set; }
    }
}
