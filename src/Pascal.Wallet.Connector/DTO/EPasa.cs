// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Contains information about an Epasa</summary>
    /// <remarks><see href="https://github.com/PascalCoin/PascalCoin/blob/master/CHANGELOG.md">Documentation at Wallet v5.4 CHANGELOG</see></remarks>
    public class EPasa
    {
        /// <summary>Encoded EPASA with extended checksum</summary>
        [JsonPropertyName("account_epasa")]
        public string AccountEPasa { get; set; }

        /// <summary>Account number</summary>
        [JsonPropertyName("account")]
        public uint AccountNumber { get; set; }

        /// <summary>Encode type of the item payload</summary>
        [JsonPropertyName("payload_method")]
        public PayloadMethod PayloadMethod { get; set; }

        /// <summary>Provided only if PayloadMethod = "aes"</summary>
        [JsonPropertyName("pwd")]
        public string Password { get; set; }

        /// <summary>Payload Encoding</summary>
        [JsonPropertyName("payload_encode")]
        public PayloadEncode PayloadEncode { get; set; }

        /// <summary>"Encoded EPASA without extended checksum</summary>
        [JsonPropertyName("account_epasa_classic")]
        public string AccountEPasaClassic { get; set; }

        /// <summary>HEXASTRING with the payload data</summary>
        [JsonPropertyName("payload")]
        public string PayloadHexString { get; set; }

        /// <summary>String with the payload data</summary>
        public string Payload { get => PayloadHexString?.FromHexString(); }

        /// <summary>The PayloadType describes the encryption and encoding of the Payload.</summary>
        [JsonPropertyName("payload_type")]
        public PayloadType PayloadType { get; set; }

        /// <summary>True if EPasa is a Pay To Key format like @[Base58Pubkey]</summary>
        [JsonPropertyName("is_pay_to_key")]
        public bool IsPayToKey { get; set; }
    }
}
