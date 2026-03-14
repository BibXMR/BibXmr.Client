using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Wallet.Dto;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ValidateAddress</c>.
    /// </summary>
    internal sealed class ValidateAddressResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ValidateAddress? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>IntegratedAddress</c>.
    /// </summary>
    internal sealed class IntegratedAddressResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public IntegratedAddress? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>BulkPayments</c>.
    /// </summary>
    internal sealed class BulkPaymentsResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public BulkPaymentsResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>BulkPayments</c>.
    /// </summary>
    internal sealed class BulkPaymentsResult
    {
        /// <summary>
        /// Gets or sets the collection of payments.
        /// </summary>
        [JsonPropertyName("payments")]
        public List<PaymentDetail> Payments { get; set; } = [];
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>FrozenOutput</c>.
    /// </summary>
    internal sealed class FrozenOutputResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public FrozenOutputResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>FrozenOutput</c>.
    /// </summary>
    internal sealed class FrozenOutputResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether frozen.
        /// </summary>
        [JsonPropertyName("frozen")]
        public bool Frozen { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>DefaultFeePriority</c>.
    /// </summary>
    internal sealed class DefaultFeePriorityResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public DefaultFeePriorityResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>DefaultFeePriority</c>.
    /// </summary>
    internal sealed class DefaultFeePriorityResult
    {
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        [JsonPropertyName("priority")]
        public uint Priority { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>TransactionProof</c>.
    /// </summary>
    internal sealed class TransactionProofResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public TransactionProofResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>TransactionProof</c>.
    /// </summary>
    internal sealed class TransactionProofResult
    {
        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        [JsonPropertyName("signature")]
        public string? Signature { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>CheckTransactionProof</c>.
    /// </summary>
    internal sealed class CheckTransactionProofResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CheckTransactionProof? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>SpendProof</c>.
    /// </summary>
    internal sealed class SpendProofResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SpendProofResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>SpendProof</c>.
    /// </summary>
    internal sealed class SpendProofResult
    {
        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        [JsonPropertyName("signature")]
        public string? Signature { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>CheckSpendProof</c>.
    /// </summary>
    internal sealed class CheckSpendProofResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CheckSpendProofResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>CheckSpendProof</c>.
    /// </summary>
    internal sealed class CheckSpendProofResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether good.
        /// </summary>
        [JsonPropertyName("good")]
        public bool Good { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>ReserveProof</c>.
    /// </summary>
    internal sealed class ReserveProofResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ReserveProofResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>ReserveProof</c>.
    /// </summary>
    internal sealed class ReserveProofResult
    {
        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        [JsonPropertyName("signature")]
        public string? Signature { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>CheckReserveProof</c>.
    /// </summary>
    internal sealed class CheckReserveProofResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CheckReserveProof? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>ExchangeMultiSigKeys</c>.
    /// </summary>
    internal sealed class ExchangeMultiSigKeysResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ExchangeMultiSigKeys? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>EstimateTxSizeAndWeight</c>.
    /// </summary>
    internal sealed class EstimateTxSizeAndWeightResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public EstimateTxSizeAndWeight? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>GenerateFromKeysWallet</c>.
    /// </summary>
    internal sealed class GenerateFromKeysWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public GenerateFromKeysWallet? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>RestoreDeterministicWallet</c>.
    /// </summary>
    internal sealed class RestoreDeterministicWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public RestoreDeterministicWallet? Result { get; set; }
    }
}


