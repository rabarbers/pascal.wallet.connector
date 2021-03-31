// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    public class AccountKeyPair
    {
        public AccountKeyPair(uint accountNumber, string encodedPublicKey = null, string b58PublicKey = null)
        {
            AccountNumber = accountNumber;
            EncodedPublicKey = encodedPublicKey;
            B58PublicKey = b58PublicKey;
        }

        /// <summary>Account that will sign</summary>
        [JsonPropertyName("account")]
        public uint AccountNumber { get; set; }

        /// <summary>Public key that will sign in encoded format. This HEXASTRING has no checksum, so, if using it always must be sure that value is correct.</summary>
        [JsonPropertyName("enc_pubkey")]
        public string EncodedPublicKey { get; set; }

        /// <summary>Public key that will sign in Base 58 format, also contains a checksum. This is the same value that Application Wallet exports as a public key</summary>
        [JsonPropertyName("b58_pubkey")]
        public string B58PublicKey { get; set; }
    }
}
