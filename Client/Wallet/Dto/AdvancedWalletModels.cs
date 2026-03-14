using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for validate address.
    /// </summary>
    public sealed class ValidateAddress
    {
        /// <summary>
        /// Gets or sets a value indicating whether valid.
        /// </summary>
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether integrated.
        /// </summary>
        [JsonPropertyName("integrated")]
        public bool Integrated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether subaddress.
        /// </summary>
        [JsonPropertyName("subaddress")]
        public bool Subaddress { get; set; }

        /// <summary>
        /// Gets or sets the net type.
        /// </summary>
        [JsonPropertyName("nettype")]
        public string? NetType { get; set; }

        /// <summary>
        /// Gets or sets the open alias address.
        /// </summary>
        [JsonPropertyName("openalias_address")]
        public string? OpenAliasAddress { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for integrated address.
    /// </summary>
    public sealed class IntegratedAddress
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("integrated_address")]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the standard address.
        /// </summary>
        [JsonPropertyName("standard_address")]
        public string? StandardAddress { get; set; }

        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        [JsonPropertyName("payment_id")]
        public string? PaymentId { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for check transaction proof.
    /// </summary>
    public sealed class CheckTransactionProof
    {
        /// <summary>
        /// Gets or sets a value indicating whether good.
        /// </summary>
        [JsonPropertyName("good")]
        public bool Good { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in pool.
        /// </summary>
        [JsonPropertyName("in_pool")]
        public bool InPool { get; set; }

        /// <summary>
        /// Gets or sets the received.
        /// </summary>
        [JsonPropertyName("received")]
        public ulong Received { get; set; }

        /// <summary>
        /// Gets or sets the confirmations.
        /// </summary>
        [JsonPropertyName("confirmations")]
        public ulong Confirmations { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for check reserve proof.
    /// </summary>
    public sealed class CheckReserveProof
    {
        /// <summary>
        /// Gets or sets a value indicating whether good.
        /// </summary>
        [JsonPropertyName("good")]
        public bool Good { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        [JsonPropertyName("total")]
        public ulong Total { get; set; }

        /// <summary>
        /// Gets or sets the spent.
        /// </summary>
        [JsonPropertyName("spent")]
        public ulong Spent { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for monero daemon connection.
    /// </summary>
    public sealed class MoneroDaemonConnection
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trusted.
        /// </summary>
        public bool? Trusted { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for exchange multi sig keys.
    /// </summary>
    public sealed class ExchangeMultiSigKeys
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the multisig info.
        /// </summary>
        [JsonPropertyName("multisig_info")]
        public string? MultisigInfo { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for estimate tx size and weight.
    /// </summary>
    public sealed class EstimateTxSizeAndWeight
    {
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [JsonPropertyName("size")]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        [JsonPropertyName("weight")]
        public ulong Weight { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for monero wallet config.
    /// </summary>
    public sealed class MoneroWalletConfig
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        public string? Seed { get; set; }

        /// <summary>
        /// Gets or sets the seed offset.
        /// </summary>
        public string? SeedOffset { get; set; }

        /// <summary>
        /// Gets or sets the primary address.
        /// </summary>
        public string? PrimaryAddress { get; set; }

        /// <summary>
        /// Gets or sets the private view key.
        /// </summary>
        public string? PrivateViewKey { get; set; }

        /// <summary>
        /// Gets or sets the private spend key.
        /// </summary>
        public string? PrivateSpendKey { get; set; }

        /// <summary>
        /// Gets or sets the restore height.
        /// </summary>
        public uint? RestoreHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether save current.
        /// </summary>
        public bool? SaveCurrent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable multisig experimental.
        /// </summary>
        public bool? EnableMultisigExperimental { get; set; }
    }

    /// <summary>
    /// Builds monero wallet config builder instances.
    /// </summary>
    public sealed class MoneroWalletConfigBuilder
    {
        /// <summary>
        /// The wallet configuration being built by this builder.
        /// </summary>
        private readonly MoneroWalletConfig _config = new();

        /// <summary>
        /// Sets filename and returns the current builder.
        /// </summary>
        /// <param name="filename">The wallet file name.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithFilename(string filename)
        {
            _config.Filename = filename;
            return this;
        }

        /// <summary>
        /// Sets password and returns the current builder.
        /// </summary>
        /// <param name="password">The wallet password.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithPassword(string? password)
        {
            _config.Password = password;
            return this;
        }

        /// <summary>
        /// Sets language and returns the current builder.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithLanguage(string? language)
        {
            _config.Language = language;
            return this;
        }

        /// <summary>
        /// Sets seed and returns the current builder.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithSeed(string? seed)
        {
            _config.Seed = seed;
            return this;
        }

        /// <summary>
        /// Sets seed offset and returns the current builder.
        /// </summary>
        /// <param name="seedOffset">The seed offset.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithSeedOffset(string? seedOffset)
        {
            _config.SeedOffset = seedOffset;
            return this;
        }

        /// <summary>
        /// Sets primary address and returns the current builder.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithPrimaryAddress(string? address)
        {
            _config.PrimaryAddress = address;
            return this;
        }

        /// <summary>
        /// Sets private view key and returns the current builder.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithPrivateViewKey(string? key)
        {
            _config.PrivateViewKey = key;
            return this;
        }

        /// <summary>
        /// Sets private spend key and returns the current builder.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithPrivateSpendKey(string? key)
        {
            _config.PrivateSpendKey = key;
            return this;
        }

        /// <summary>
        /// Sets restore height and returns the current builder.
        /// </summary>
        /// <param name="restoreHeight">The restore height.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithRestoreHeight(uint? restoreHeight)
        {
            _config.RestoreHeight = restoreHeight;
            return this;
        }

        /// <summary>
        /// Sets save current and returns the current builder.
        /// </summary>
        /// <param name="saveCurrent">The save current.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithSaveCurrent(bool? saveCurrent)
        {
            _config.SaveCurrent = saveCurrent;
            return this;
        }

        /// <summary>
        /// Sets enable multisig experimental and returns the current builder.
        /// </summary>
        /// <param name="enabled">The enabled.</param>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfigBuilder WithEnableMultisigExperimental(bool? enabled)
        {
            _config.EnableMultisigExperimental = enabled;
            return this;
        }

        /// <summary>
        /// Executes the build operation.
        /// </summary>
        /// <returns>The operation result.</returns>
        public MoneroWalletConfig Build() => _config;
    }

    /// <summary>
    /// Represents wallet RPC data for monero tx destination.
    /// </summary>
    public sealed class MoneroTxDestination
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for monero tx config.
    /// </summary>
    public sealed class MoneroTxConfig
    {
        /// <summary>
        /// Gets or sets the collection of destinations.
        /// </summary>
        public List<MoneroTxDestination> Destinations { get; set; } = [];

        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        public string? PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public TransferPriority? Priority { get; set; }

        /// <summary>
        /// Gets or sets the account index.
        /// </summary>
        public uint? AccountIndex { get; set; }

        /// <summary>
        /// Gets or sets the collection of subaddress indices.
        /// </summary>
        public List<uint>? SubaddressIndices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relay.
        /// </summary>
        public bool? Relay { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Gets or sets the recipient name.
        /// </summary>
        public string? RecipientName { get; set; }

        /// <summary>
        /// Gets or sets the below amount.
        /// </summary>
        public ulong? BelowAmount { get; set; }

        /// <summary>
        /// Gets or sets the key image.
        /// </summary>
        public string? KeyImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether do not relay.
        /// </summary>
        public bool? DoNotRelay { get; set; }

        /// <summary>
        /// Gets or sets the unlock time.
        /// </summary>
        public ulong? UnlockTime { get; set; }
    }

    /// <summary>
    /// Builds monero tx config builder instances.
    /// </summary>
    public sealed class MoneroTxConfigBuilder
    {
        /// <summary>
        /// The transaction configuration being built by this builder.
        /// </summary>
        private readonly MoneroTxConfig _config = new();

        /// <summary>
        /// Adds destination.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder AddDestination(string address, ulong amount)
        {
            _config.Destinations.Add(new MoneroTxDestination { Address = address, Amount = amount });
            return this;
        }

        /// <summary>
        /// Sets payment id and returns the current builder.
        /// </summary>
        /// <param name="paymentId">The payment id.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithPaymentId(string? paymentId)
        {
            _config.PaymentId = paymentId;
            return this;
        }

        /// <summary>
        /// Sets priority and returns the current builder.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithPriority(TransferPriority? priority)
        {
            _config.Priority = priority;
            return this;
        }

        /// <summary>
        /// Sets account index and returns the current builder.
        /// </summary>
        /// <param name="accountIndex">The account index.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithAccountIndex(uint? accountIndex)
        {
            _config.AccountIndex = accountIndex;
            return this;
        }

        /// <summary>
        /// Sets subaddress indices and returns the current builder.
        /// </summary>
        /// <param name="subaddressIndices">The subaddress indices.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithSubaddressIndices(List<uint>? subaddressIndices)
        {
            _config.SubaddressIndices = subaddressIndices;
            return this;
        }

        /// <summary>
        /// Sets relay and returns the current builder.
        /// </summary>
        /// <param name="relay">The relay.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithRelay(bool? relay)
        {
            _config.Relay = relay;
            return this;
        }

        /// <summary>
        /// Sets note and returns the current builder.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithNote(string? note)
        {
            _config.Note = note;
            return this;
        }

        /// <summary>
        /// Sets recipient name and returns the current builder.
        /// </summary>
        /// <param name="recipientName">The recipient name.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithRecipientName(string? recipientName)
        {
            _config.RecipientName = recipientName;
            return this;
        }

        /// <summary>
        /// Sets below amount and returns the current builder.
        /// </summary>
        /// <param name="belowAmount">The below amount.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithBelowAmount(ulong? belowAmount)
        {
            _config.BelowAmount = belowAmount;
            return this;
        }

        /// <summary>
        /// Sets key image and returns the current builder.
        /// </summary>
        /// <param name="keyImage">The key image.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithKeyImage(string? keyImage)
        {
            _config.KeyImage = keyImage;
            return this;
        }

        /// <summary>
        /// Sets do not relay and returns the current builder.
        /// </summary>
        /// <param name="doNotRelay">The do not relay.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithDoNotRelay(bool? doNotRelay)
        {
            _config.DoNotRelay = doNotRelay;
            return this;
        }

        /// <summary>
        /// Sets unlock time and returns the current builder.
        /// </summary>
        /// <param name="unlockTime">The unlock time.</param>
        /// <returns>The operation result.</returns>
        public MoneroTxConfigBuilder WithUnlockTime(ulong? unlockTime)
        {
            _config.UnlockTime = unlockTime;
            return this;
        }

        /// <summary>
        /// Executes the build operation.
        /// </summary>
        /// <returns>The operation result.</returns>
        public MoneroTxConfig Build() => _config;
    }

    /// <summary>
    /// Specifies options for generate from keys options operations.
    /// </summary>
    public sealed class GenerateFromKeysOptions
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the view key.
        /// </summary>
        public string ViewKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the spend key.
        /// </summary>
        public string? SpendKey { get; set; }

        /// <summary>
        /// Gets or sets the restore height.
        /// </summary>
        public uint? RestoreHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether save current.
        /// </summary>
        public bool? SaveCurrent { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for generate from keys wallet.
    /// </summary>
    public sealed class GenerateFromKeysWallet
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        [JsonPropertyName("info")]
        public string? Info { get; set; }
    }

    /// <summary>
    /// Specifies options for restore deterministic wallet options operations.
    /// </summary>
    public sealed class RestoreDeterministicWalletOptions
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        public string Seed { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the seed offset.
        /// </summary>
        public string? SeedOffset { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets the restore height.
        /// </summary>
        public uint? RestoreHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether save current.
        /// </summary>
        public bool? SaveCurrent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable multisig experimental.
        /// </summary>
        public bool? EnableMultisigExperimental { get; set; }
    }

    /// <summary>
    /// Represents wallet RPC data for restore deterministic wallet.
    /// </summary>
    public sealed class RestoreDeterministicWallet
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        [JsonPropertyName("seed")]
        public string? Seed { get; set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        [JsonPropertyName("info")]
        public string? Info { get; set; }
    }
}


