// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#payloaddecrypt">https://www.pascalcoin.org/development/rpc#payloaddecrypt</see></remarks>
    public class EncryptionResult
    {
        /// <summary>True if decryption was successful, otherwise - false</summary>
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        /// <summary>HEXASTRING - Same value than param payload sent</summary>
        [JsonPropertyName("enc_payload")]
        public string EncryptedPayload { get; set; }

        /// <summary>Unencoded value in readable format(no HEXASTRING)</summary>
        [JsonPropertyName("unenc_payload")]
        public string UnencryptedPayload { get; set; }

        /// <summary>HEXASTRING - Unencoded value in hexastring</summary>
        [JsonPropertyName("unenc_hexpayload")]
        public string UnencryptedPayloadHexastring { get; set; }

        /// <summary>"key" or "pwd"</summary>
        [JsonPropertyName("payload_method")]
        public string PayloadMethod { get; set; }

        /// <summary>HEXASTRING - Encoded public key used to decrypt when method = "key"</summary>
        [JsonPropertyName("enc_pubkey")]
        public string EncodedPublicKey { get; set; }

        /// <summary>String value used to decrypt when method = "pwd"</summary>
        [JsonPropertyName("pwd")]
        public string Password { get; set; }
    }
}
