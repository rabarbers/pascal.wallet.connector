// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary><see href="https://www.pascalcoin.org/development/rpc#sendto">https://www.pascalcoin.org/development/rpc#sendto</see></summary>
    public enum PayloadMethod
    {
        /// <summary>No encryption. Will be visible for everybody.</summary>
        None,
        /// <summary>Using Public key of "target" account. Only "target" will be able to decrypt this payload</summary>
        Dest,
        /// <summary>Using sender Public key. Only "sender" will be able to decrypt this payload</summary>
        Sender,
        /// <summary>Encrypted data using pwd param</summary>
        Aes
    }

    /// <summary><see href="https://www.pascalcoin.org/development/rpc#payloaddecrypt">https://www.pascalcoin.org/development/rpc#payloaddecrypt</see></summary>
    public enum AbstractPayloadMethod
    {
        /// <summary>No encryption. Will be visible for everybody.</summary>
        None,
        /// <summary>Using a Publick Key. Only owner of this private key will be able to read it. Must provide enc_pubkey or b58_pubkey param. See decodepubkey or encodepubkey.</summary>
        PubKey,
        /// <summary>Using a Password. Must provide pwd param</summary>
        Aes
    }
}
