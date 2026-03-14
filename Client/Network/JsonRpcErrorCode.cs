namespace BibXmr.Client.Network
{
    /// <summary>
    /// Specifies values for json rpc error code.
    /// </summary>
    public enum JsonRpcErrorCode
    {
        //////////////////////////////
        // JsonRpc-Related Errors   //
        //////////////////////////////

        /// <summary>
        /// Invalid JSON was received by the server.
        /// An error occurred on the server while parsing the JSON text.
        /// </summary>
        ParseError = -32700,

        /// <summary>
        /// The JSON sent is not a valid Request object.
        /// </summary>
        InvalidRequest = -32600,

        /// <summary>
        /// The method does not exist / is not available.
        /// </summary>
        MethodNotFound = -32601,

        /// <summary>
        /// Invalid method parameter(s).
        /// </summary>
        InvalidParameters = -32602,

        /// <summary>
        /// Internal JSON-RPC error.
        /// </summary>
        InternalJsonError = -32603,

        /// <summary>
        /// Reserved for implementation-defined server-errors.
        /// </summary>
        ServerError = -32000,

        /////////////////////////////
        // Monero-Related Errors   //
        /////////////////////////////

        // Source: https://github.com/monero-project/monero/blob/8286f07b265d16a87b3fe3bb53e8d7bf37b5265a/src/wallet/wallet_rpc_server_error_codes.h

        /// <summary>
        /// Represents unknown error.
        /// </summary>
        UnknownError = -1,

        /// <summary>
        /// Represents wrong address.
        /// </summary>
        WrongAddress = -2,

        /// <summary>
        /// Represents daemon is busy.
        /// </summary>
        DaemonIsBusy = -3,

        /// <summary>
        /// Represents generic transfer error.
        /// </summary>
        GenericTransferError = -4,

        /// <summary>
        /// Represents wrong payment id.
        /// </summary>
        WrongPaymentID = -5,

        /// <summary>
        /// Represents transfer type.
        /// </summary>
        TransferType = -6,

        /// <summary>
        /// Represents denied.
        /// </summary>
        Denied = -7,

        /// <summary>
        /// Represents wrong txid.
        /// </summary>
        WrongTxid = -8,

        /// <summary>
        /// Represents wrong signature.
        /// </summary>
        WrongSignature = -9,

        /// <summary>
        /// Represents wrong key image.
        /// </summary>
        WrongKeyImage = -10,

        /// <summary>
        /// Represents wrong uri.
        /// </summary>
        WrongUri = -11,

        /// <summary>
        /// Represents wrong index.
        /// </summary>
        WrongIndex = -12,

        /// <summary>
        /// Represents not open.
        /// </summary>
        NotOpen = -13,

        /// <summary>
        /// Represents account index out of bounds.
        /// </summary>
        AccountIndexOutOfBounds = -14,

        /// <summary>
        /// Represents address index out of bounds.
        /// </summary>
        AddressIndexOutOfBounds = -15,

        /// <summary>
        /// Represents tx not possible.
        /// </summary>
        TxNotPossible = -16,

        /// <summary>
        /// Represents not enough money.
        /// </summary>
        NotEnoughMoney = -17,

        /// <summary>
        /// Represents tx too large.
        /// </summary>
        TxTooLarge = -18,

        /// <summary>
        /// Represents nout enough outs to mix.
        /// </summary>
        NoutEnoughOutsToMix = -19,

        /// <summary>
        /// Represents zero destination.
        /// </summary>
        ZeroDestination = -20,

        /// <summary>
        /// Represents wallet already exists.
        /// </summary>
        WalletAlreadyExists = -21,

        /// <summary>
        /// Represents invalid password.
        /// </summary>
        InvalidPassword = -22,

        /// <summary>
        /// Represents no wallet directory.
        /// </summary>
        NoWalletDirectory = -23,

        /// <summary>
        /// Represents no tx key.
        /// </summary>
        NoTxKey = -24,

        /// <summary>
        /// Represents wrong key.
        /// </summary>
        WrongKey = -25,

        /// <summary>
        /// Represents bad hex.
        /// </summary>
        BadHex = -26,

        /// <summary>
        /// Represents bad tx metadata.
        /// </summary>
        BadTxMetadata = -27,

        /// <summary>
        /// Represents already multi sig.
        /// </summary>
        AlreadyMultiSig = -28,

        /// <summary>
        /// Represents watch only.
        /// </summary>
        WatchOnly = -29,

        /// <summary>
        /// Represents bad multi sig info.
        /// </summary>
        BadMultiSigInfo = -30,

        /// <summary>
        /// Represents not multi sig.
        /// </summary>
        NotMultiSig = -31,

        /// <summary>
        /// Represents invalid multisig LR curve point data.
        /// </summary>
        WrongLR = -32,  // MultiSig curve points that get "merged" from all signers.

        /// <summary>
        /// Represents threshold not reached.
        /// </summary>
        ThresholdNotReached = -33,

        /// <summary>
        /// Represents bad multi sig tx data.
        /// </summary>
        BadMultiSigTxData = -34,

        /// <summary>
        /// Represents multi sig signature.
        /// </summary>
        MultiSigSignature = -35,

        /// <summary>
        /// Represents multi sig submission.
        /// </summary>
        MultiSigSubmission = -36,

        /// <summary>
        /// Represents not enough unlocked money.
        /// </summary>
        NotEnoughUnlockedMoney = -37,

        /// <summary>
        /// Represents no daemon connection.
        /// </summary>
        NoDaemonConnection = -38,

        /// <summary>
        /// Represents bad unsigned tx data.
        /// </summary>
        BadUnsignedTxData = -39,

        /// <summary>
        /// Represents bad signed tx data.
        /// </summary>
        BadSignedTxData = -40,

        /// <summary>
        /// Represents signed submission.
        /// </summary>
        SignedSubmission = -41,

        /// <summary>
        /// Represents sign unsigned.
        /// </summary>
        SignUnsigned = -42,

        /// <summary>
        /// Represents non deterministic.
        /// </summary>
        NonDeterministic = -43,

        /// <summary>
        /// Represents invalid log level.
        /// </summary>
        InvalidLogLevel = -44,

        /// <summary>
        /// Represents attribute not found.
        /// </summary>
        AttributeNotFound = -45,

        /// <summary>
        /// Represents an invalid signature type.
        /// </summary>
        InvalidSignatureType = -47, // Yes, -46 appears to be missing. Maybe -46 is bad luck.
    }
}
