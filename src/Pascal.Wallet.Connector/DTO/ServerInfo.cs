// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin

using System;
using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Information about a servers</summary>
    public class ServerInfo
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("port")]
        public uint Port { get; set; }

        [JsonConverter(typeof(UnixTimeConverter))]
        [JsonPropertyName("lastcon")]
        public DateTime LastConnected { get; set; }

        [JsonPropertyName("attempts")]
        public uint Attempts { get; set; }
    }
}
