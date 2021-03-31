// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    public class NetProtocolInfo
    {
        /// <summary>Net protocol version</summary>
        [JsonPropertyName("ver")]
        public uint Version { get; set; }

        /// <summary>Available Net protocol version</summary>
        [JsonPropertyName("ver_a")]
        public uint AvailableVersion { get; set; }
    }
}
