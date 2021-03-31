// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System;
using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Contains information about node</summary>
    /// <remarks><see href="//https://www.pascalcoin.org/development/rpc#nodestatus">//https://www.pascalcoin.org/development/rpc#nodestatus</see></remarks>
    public class NodeStatus
    {
        /// <summary>Must be true, otherwise Node is not ready to execute operations</summary>
        [JsonPropertyName("ready")]
        public bool Ready { get; set; }

        /// <summary>Human readable information about node status... Running, downloading blockchain, discovering servers...</summary>
        [JsonPropertyName("status_s")]
        public string Description { get; set; }

        /// <summary>Server port</summary>
        [JsonPropertyName("port")]
        public uint Port { get; set; }

        /// <summary>True when this wallet is locked, false otherwise</summary>
        [JsonPropertyName("locked")]
        public bool Locked { get; set; }

        /// <summary>Timestamp of the Node</summary>
        [JsonConverter(typeof(UnixTimeConverter))]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>Server version</summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>Server version</summary>
        [JsonPropertyName("netprotocol")]
        public NetProtocolInfo NetProtocol { get; set; }

        /// <summary>Server version</summary>
        [JsonPropertyName("blocks")]
        public uint Blocks { get; set; }

        /// <summary>Object with net information</summary>
        [JsonPropertyName("netstats")]
        public NetStatistics NetStats { get; set; }

        /// <summary>Array with servers candidates</summary>
        [JsonPropertyName("nodeservers")]
        public ServerInfo[] NodeServers { get; set; }

        /// <summary>Safebox Hash</summary>
        [JsonPropertyName("sbh")]
        public string SafeboxHash { get; set; }

        /// <summary>Proof of work</summary>
        [JsonPropertyName("pow")]
        public string ProofOfWork { get; set; }
    }
}
