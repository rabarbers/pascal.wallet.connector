// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Information about a connection to other node</summary>
    /// <see href="https://www.pascalcoin.org/development/rpc#connection-object">https://www.pascalcoin.org/development/rpc#connection-object</see>
    public class ConnectionInfo
    {
        /// <summary>True if this connection is to a server node.False if this connection is a client node</summary>
        [JsonPropertyName("server")]
        public bool Server { get; set; }

        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("port")]
        public uint Port { get; set; }

        /// <summary>Seconds of live of this connection</summary>
        [JsonPropertyName("secs")]
        public uint Seconds { get; set; }

        [JsonPropertyName("sent")]
        public uint BytesSent { get; set; }

        [JsonPropertyName("recv")]
        public uint BytesReceived { get; set; }

        /// <summary>Other node App version</summary>
        [JsonPropertyName("appver")]
        public string AppVersion { get; set; }

        /// <summary>Net protocol of other node</summary>
        [JsonPropertyName("netver")]
        public uint NetVersion { get; set; }

        /// <summary>Net protocol available of other node</summary>
        [JsonPropertyName("netver_a")]
        public uint NetAvailableVersion { get; set; }

        /// <summary>Net timediff of other node (vs wallet)</summary>
        [JsonPropertyName("timediff")]
        public int TimeDifference { get; set; }
    }
}
