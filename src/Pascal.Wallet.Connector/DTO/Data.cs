// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Data structure used in DataOperations</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/pips/pip-0016">https://www.pascalcoin.org/development/pips/pip-0016</see></remarks>
    public class Data
    {
        /// <summary>GUID created by the sender</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>Sequence is for chaining multiple data packets together into a logical blob</summary>
        [JsonPropertyName("sequence")]
        public uint Sequence { get; set; }

        /// <summary>Type of the message</summary>
        [JsonPropertyName("type")]
        public DataType Type { get; set; }
    }
}
