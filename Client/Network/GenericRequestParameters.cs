using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Requests;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Requests;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Used for json_rpc interface commands.
    /// </summary>
    internal class GenericRequestParameters
    {
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public ulong? Height { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        public string? Hash { get; set; }

        /// <summary>
        /// Gets or sets the start height.
        /// </summary>
        public uint? Start_height { get; set; }

        /// <summary>
        /// Gets or sets the end height.
        /// </summary>
        public uint? End_height { get; set; }

        /// <summary>
        /// Gets or sets the restore height.
        /// </summary>
        public uint? Restore_height { get; set; }

        /// <summary>
        /// Gets or sets the collection of txids.
        /// </summary>
        public IEnumerable<string>? Txids { get; set; }

        /// <summary>
        /// Gets or sets the collection of amounts.
        /// </summary>
        public IEnumerable<ulong>? Amounts { get; set; }

        /// <summary>
        /// Gets or sets the min count.
        /// </summary>
        public uint? Min_count { get; set; }

        /// <summary>
        /// Gets or sets the max count.
        /// </summary>
        public uint? Max_count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unlocked.
        /// </summary>
        public bool? Unlocked { get; set; }

        /// <summary>
        /// Gets or sets the recent cutoff.
        /// </summary>
        public uint? Recent_cutoff { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public uint? Count { get; set; }

        /// <summary>
        /// Gets or sets the grace blocks.
        /// </summary>
        public uint? Grace_blocks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cumulative.
        /// </summary>
        public bool? Cumulative { get; set; }

        /// <summary>
        /// Gets or sets the from height.
        /// </summary>
        public ulong? From_height { get; set; }

        /// <summary>
        /// Gets or sets the to height.
        /// </summary>
        public ulong? To_height { get; set; }

        /// <summary>
        /// Gets or sets the collection of bans.
        /// </summary>
        public List<NodeBan>? Bans { get; set; }

        /// <summary>
        /// Gets or sets the account index.
        /// </summary>
        public uint? Account_index { get; set; }

        /// <summary>
        /// Gets or sets the collection of address indices.
        /// </summary>
        public IEnumerable<uint>? Address_indices { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public ulong? Amount { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the tx description.
        /// </summary>
        public string? Tx_description { get; set; }

        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        public string? Payment_id { get; set; }

        /// <summary>
        /// Gets or sets the recipient name.
        /// </summary>
        public string? Recipient_name { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public object? Index { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public string? Tag { get; set; }

        /// <summary>
        /// Gets or sets the collection of entries.
        /// </summary>
        public IEnumerable<uint>? Entries { get; set; }

        /// <summary>
        /// Gets or sets the collection of accounts.
        /// </summary>
        public IEnumerable<uint>? Accounts { get; set; }

        /// <summary>
        /// Gets or sets the collection of multisig info.
        /// </summary>
        public IEnumerable<string>? Multisig_info { get; set; }

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        public uint? Threshold { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the collection of destinations.
        /// </summary>
        public IEnumerable<FundTransferParameter>? Destinations { get; set; }

        /// <summary>
        /// Gets or sets the collection of subaddr indices.
        /// </summary>
        public IEnumerable<uint>? Subaddr_indices { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public uint? Priority { get; set; }

        /// <summary>
        /// Gets or sets the mixin.
        /// </summary>
        public uint? Mixin { get; set; }

        /// <summary>
        /// Gets or sets the ring size.
        /// </summary>
        public uint? Ring_size { get; set; }

        /// <summary>
        /// Gets or sets the unlock time.
        /// </summary>
        public ulong? Unlock_time { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether get tx key.
        /// </summary>
        public bool? Get_tx_key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether get tx keys.
        /// </summary>
        public bool? Get_tx_keys { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether get tx hex.
        /// </summary>
        public bool? Get_tx_hex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether get tx metadata.
        /// </summary>
        public bool? Get_tx_metadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether include txid.
        /// </summary>
        [JsonPropertyName("get_txid")]
        public bool? Include_txid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether new algorithm.
        /// </summary>
        public bool? New_algorithm { get; set; }

        /// <summary>
        /// Gets or sets the unsigned txset.
        /// </summary>
        public string? Unsigned_txset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether export raw.
        /// </summary>
        public bool? Export_raw { get; set; }

        /// <summary>
        /// Gets or sets the tx data hex.
        /// </summary>
        public string? Tx_data_hex { get; set; }

        /// <summary>
        /// Gets or sets the transaction key.
        /// </summary>
        [JsonPropertyName("tx_key")]
        public string? Transaction_key { get; set; }

        /// <summary>
        /// Gets or sets the transaction metadata.
        /// </summary>
        [JsonPropertyName("tx_metadata")]
        public string? Transaction_metadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether do not relay.
        /// </summary>
        public bool? Do_not_relay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relay.
        /// </summary>
        public bool? Relay { get; set; }

        /// <summary>
        /// Gets or sets the below amount.
        /// </summary>
        public ulong? Below_amount { get; set; }

        /// <summary>
        /// Gets or sets the transfer type.
        /// </summary>
        public string? Transfer_type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether verbose.
        /// </summary>
        public bool? Verbose { get; set; }

        /// <summary>
        /// Gets or sets the key type.
        /// </summary>
        public string? Key_type { get; set; }

        /// <summary>
        /// Gets or sets the collection of notes.
        /// </summary>
        public IEnumerable<string>? Notes { get; set; }

        /// <summary>
        /// Gets or sets the txid.
        /// </summary>
        public string? Txid { get; set; }

        /// <summary>
        /// Gets the get.
        /// </summary>
        public bool? In { get; set; }

        /// <summary>
        /// Gets the get.
        /// </summary>
        public bool? Out { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pending.
        /// </summary>
        public bool? Pending { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether failed.
        /// </summary>
        public bool? Failed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pool.
        /// </summary>
        public bool? Pool { get; set; }

        /// <summary>
        /// Gets or sets the min height.
        /// </summary>
        public ulong? Min_height { get; set; }

        /// <summary>
        /// Gets or sets the max height.
        /// </summary>
        public ulong? Max_height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether filter by height.
        /// </summary>
        public bool? Filter_by_height { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        public string? Signature { get; set; }

        /// <summary>
        /// Gets or sets the collection of signed key images.
        /// </summary>
        public List<SignedKeyImage>? Signed_key_images { get; set; }

        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        public string? Uri { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        public string? Old_password { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string? New_password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all accounts.
        /// </summary>
        public bool? All_accounts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether strict.
        /// </summary>
        public bool? Strict { get; set; }

        /// <summary>
        /// Gets or sets the multisig txset.
        /// </summary>
        public string? Multisig_txset { get; set; }

        /// <summary>
        /// Gets or sets the hex.
        /// </summary>
        public string? Hex { get; set; }

        /// <summary>
        /// Gets or sets the reserve size.
        /// </summary>
        public ulong? Reserve_size { get; set; }

        /// <summary>
        /// Gets or sets the wallet address.
        /// </summary>
        public string? Wallet_address { get; set; }

        /// <summary>
        /// Gets or sets the prev block.
        /// </summary>
        public string? Prev_block { get; set; }

        /// <summary>
        /// Gets or sets the extra nonce.
        /// </summary>
        public string? Extra_nonce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether binary.
        /// </summary>
        public bool? Binary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether compress.
        /// </summary>
        public bool? Compress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether check.
        /// </summary>
        public bool? Check { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether any net type.
        /// </summary>
        public bool? Any_net_type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow openalias.
        /// </summary>
        public bool? Allow_openalias { get; set; }

        /// <summary>
        /// Gets or sets the standard address.
        /// </summary>
        public string? Standard_address { get; set; }

        /// <summary>
        /// Gets or sets the integrated address.
        /// </summary>
        public string? Integrated_address { get; set; }

        /// <summary>
        /// Gets or sets the collection of payment ids.
        /// </summary>
        public IEnumerable<string>? Payment_ids { get; set; }

        /// <summary>
        /// Gets or sets the min block height.
        /// </summary>
        public ulong? Min_block_height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable.
        /// </summary>
        public bool? Enable { get; set; }

        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        public uint? Period { get; set; }

        /// <summary>
        /// Gets or sets the threads count.
        /// </summary>
        public ulong? Threads_count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether do background mining.
        /// </summary>
        public bool? Do_background_mining { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignore battery.
        /// </summary>
        public bool? Ignore_battery { get; set; }

        /// <summary>
        /// Gets or sets the key image.
        /// </summary>
        public string? Key_image { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trusted.
        /// </summary>
        public bool? Trusted { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the multisig hex.
        /// </summary>
        public string? Multisig_hex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all.
        /// </summary>
        public bool? All { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether decode as json.
        /// </summary>
        public bool? Decode_as_json { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prune.
        /// </summary>
        public bool? Prune { get; set; }

        /// <summary>
        /// Gets or sets the collection of key images.
        /// </summary>
        public IEnumerable<string>? Key_images { get; set; }

        /// <summary>
        /// Gets or sets the collection of outputs.
        /// </summary>
        public IEnumerable<DaemonOutputRequest>? Outputs { get; set; }

        /// <summary>
        /// Gets or sets the collection of request.
        /// </summary>
        public IEnumerable<string>? Request { get; set; }

        /// <summary>
        /// Gets or sets the limit down.
        /// </summary>
        public int? Limit_down { get; set; }

        /// <summary>
        /// Gets or sets the limit up.
        /// </summary>
        public int? Limit_up { get; set; }

        /// <summary>
        /// Gets or sets the in peers.
        /// </summary>
        public int? In_peers { get; set; }

        /// <summary>
        /// Gets or sets the out peers.
        /// </summary>
        public int? Out_peers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether set.
        /// </summary>
        public bool? Set { get; set; }

        /// <summary>
        /// Gets or sets the miner address.
        /// </summary>
        public string? Miner_address { get; set; }

        /// <summary>
        /// Gets or sets the tx as hex.
        /// </summary>
        public string? Tx_as_hex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether do sanity checks.
        /// </summary>
        public bool? Do_sanity_checks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether set address.
        /// </summary>
        [JsonPropertyName("set_address")]
        public bool? Should_set_address { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether set description.
        /// </summary>
        [JsonPropertyName("set_description")]
        public bool? Should_set_description { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public string? Command { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gray.
        /// </summary>
        public bool? Gray { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether white.
        /// </summary>
        public bool? White { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether include blocked.
        /// </summary>
        public bool? Include_blocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether visible.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public uint? Level { get; set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        public string? Categories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bad txs.
        /// </summary>
        public bool? Bad_txs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bad blocks.
        /// </summary>
        public bool? Bad_blocks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tx proof.
        /// </summary>
        public bool? Tx_proof { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether spend proof.
        /// </summary>
        public bool? Spend_proof { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reserve proof.
        /// </summary>
        public bool? Reserve_proof { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether setup background sync.
        /// </summary>
        public bool? Setup_background_sync { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start background sync.
        /// </summary>
        public bool? Start_background_sync { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stop background sync.
        /// </summary>
        public bool? Stop_background_sync { get; set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        public string? Seed { get; set; }

        /// <summary>
        /// Gets or sets the seed offset.
        /// </summary>
        public string? Seed_offset { get; set; }

        /// <summary>
        /// Gets or sets the viewkey.
        /// </summary>
        public string? Viewkey { get; set; }

        /// <summary>
        /// Gets or sets the spendkey.
        /// </summary>
        public string? Spendkey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether autosave current.
        /// </summary>
        public bool? Autosave_current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable multisig experimental.
        /// </summary>
        public bool? Enable_multisig_experimental { get; set; }

        /// <summary>
        /// Gets or sets the index raw.
        /// </summary>
        public uint? Index_raw { get; set; }
    }
}
