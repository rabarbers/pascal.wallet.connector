// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Contains the information about a public key</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#public-key-object">https://www.pascalcoin.org/development/rpc#public-key-object</see></remarks>
    public class PublicKey
    {
        /// <summary>Encoded value of this public key.This HEXASTRING has no checksum, so, if using it always must be sure that value is correct</summary>
        [JsonPropertyName("enc_pubkey")]
        public string EncodedPublicKey { get; set; }

        /// <summary>Encoded value of this public key in Base 58 format, also contains a checksum. This is the same value that Application Wallet exports as a public key</summary>
        [JsonPropertyName("b58_pubkey")]
        public string B58PublicKey { get; set; }

        /// <summary>Indicates which EC type is used</summary>
        [JsonPropertyName("ec_nid")]
        public EncryptionType EncryptionNid { get; set; }

        /// <summary>HEXASTRING with x value of public key</summary>
        [JsonPropertyName("x")]
        public string X { get; set; }

        /// <summary>HEXASTRING with y value of public key</summary>
        [JsonPropertyName("y")]
        public string Y { get; set; }
    }

    /// <summary>Contains the information about wallet's public key</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#public-key-object">https://www.pascalcoin.org/development/rpc#public-key-object</see></remarks>
    public class WalletPublicKey: PublicKey
    {
        /// <summary>Human readable name stored at the Wallet for this key</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>If false then Wallet doesn't have Private key for this public key, so, Wallet cannot execute operations with this key</summary>
        [JsonPropertyName("can_use")]
        public bool CanUse { get; set; }
    }

}
