// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Payload Encoding</summary>
    /// <remarks><see href="https://github.com/PascalCoin/PascalCoin/blob/master/CHANGELOG.md">https://github.com/PascalCoin/PascalCoin/blob/master/CHANGELOG.md</see></remarks>
    public enum PayloadEncode
    {
        [JsonPropertyName("string")]
        String,
        [JsonPropertyName("hexa")]
        Hexa,
        [JsonPropertyName("base58")]
        Base58
    }
}
