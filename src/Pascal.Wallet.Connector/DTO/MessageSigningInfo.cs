// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Describes the result of a multi-operation signature or verification</summary>
    public class MessageSigningInfo
    {
        /// <summary>HEXASTRING with the message to sign</summary>
        [JsonPropertyName("digest")]
        public string DigestHexString { get; set; }

        public string Digest
        {
            get => DigestHexString?.FromHexString();
        }

        /// <summary>HESATRING with the public key that used to sign "digest" data</summary>
        [JsonPropertyName("enc_pubkey")]
        public string EncodedPublicKey { get; set; }

        /// <summary>HEXASTRING with signature</summary>
        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }
}
