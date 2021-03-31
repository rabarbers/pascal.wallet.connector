// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Information about node protocol support</summary>
    public class NetStatistics
    {
        [JsonPropertyName("active")]
        public uint Active { get; set; }

        [JsonPropertyName("clients")]
        public uint Clients { get; set; }

        [JsonPropertyName("servers")]
        public uint Servers { get; set; }

        [JsonPropertyName("servers_t")]
        public uint ServersT { get; set; }

        [JsonPropertyName("total")]
        public uint Total { get; set; }

        [JsonPropertyName("tclients")]
        public uint TClients { get; set; }

        [JsonPropertyName("tservers")]
        public uint TServers { get; set; }

        [JsonPropertyName("breceived")]
        public uint BytesReceived { get; set; }

        [JsonPropertyName("bsend")]
        public uint BytesSend { get; set; }

    }
}
