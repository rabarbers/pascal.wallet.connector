// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System;
using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary><see href="https://www.pascalcoin.org/development/rpc#block-object">https://www.pascalcoin.org/development/rpc#block-object</see></summary>
    public class Block
    {
        /// <summary>Block number</summary>
        [JsonPropertyName("block")]
        public uint BlockNumber { get; set; }

        /// <summary>HEXASTRING - Encoded public key value used to init 5 created accounts of this block (See decodepubkey)</summary>
        [JsonPropertyName("enc_pubkey")]
        public string EncodedPublicKey { get; set; }

        /// <summary>Reward of first account's block</summary>
        [JsonPropertyName("reward")]
        public decimal Reward { get; set; }

        /// <summary>Fee obtained by operations</summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }

        /// <summary>Pascal Coin protocol used</summary>
        [JsonPropertyName("ver")]
        public uint Version { get; set; }

        /// <summary>Pascal Coin protocol available by the miner</summary>
        [JsonPropertyName("ver_a")]
        public uint MinerVersion { get; set; }

        /// <summary>Block creation UTC time</summary>
        [JsonConverter(typeof(UnixTimeConverter))]
        [JsonPropertyName("timestamp")]
        public DateTime Time { get; set; }

        /// <summary>Target used</summary>
        [JsonPropertyName("target")]
        public uint Target { get; set; }

        //TODO: PascalCoin walled has bug converting this field to JSON format. Investigate if returned value is correct.
        /// <summary>Nonce used</summary>
        [JsonPropertyName("nonce")]
        public ulong Nonce { get; set; }

        /// <summary>Miner's payload</summary>
        [JsonPropertyName("payload")]
        public string Payload { get; set; }

        /// <summary>SafeBox Hash</summary>
        [JsonPropertyName("sbh")]
        public string SafeboxHash { get; set; }

        /// <summary>Operations hash</summary>
        [JsonPropertyName("oph")]
        public string OperationsHash { get; set; }

        /// <summary>Proof of work</summary>
        [JsonPropertyName("pow")]
        public string ProofOfWork { get; set; }

        /// <summary>Number of operations included in this block</summary>
        [JsonPropertyName("operations")]
        public uint Operations { get; set; }

        /// <summary>Estimated network hashrate calculated by previous 50 blocks average</summary>
        [JsonPropertyName("hashratekhs")]
        public uint HashRateKhs { get; set; }

        /// <summary>Number of blocks in the blockchain higher than this</summary>
        [JsonPropertyName("maturation")]
        public uint Maturation { get; set; }
    }
}
