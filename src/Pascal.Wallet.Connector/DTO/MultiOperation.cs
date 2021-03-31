// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Contains information about a multioperation.</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#multioperation-object">https://www.pascalcoin.org/development/rpc#multioperation-object</see></remarks>
    public class MultiOperation
    {
        /// <summary>HEXASTRING - Single multioperation in RAW format</summary>
        [JsonPropertyName("rawoperations")]
        public string RawOperations { get; set; }

        /// <summary>ARRAY of Sender objects</summary>
        [JsonPropertyName("senders")]
        public Sender[] Senders { get; set; }

        /// <summary>ARRAY of Receiver objects</summary>
        [JsonPropertyName("receivers")]
        public Receiver[] Receivers { get; set; }

        /// <summary>ARRAY of Changers objects</summary>
        [JsonPropertyName("changers")]
        public Changer[] Changers { get; set; }

        /// <summary>PASCURRENCY Amount received by receivers</summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>PASCURRENCY Equal to "total send" - "total received"</summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }

        /// <summary>HEXASTRING value of the digest that must be signed</summary>
        [JsonPropertyName("digest")]
        public string Digest { get; set; }

        /// <summary>Number of senders</summary>
        [JsonPropertyName("senders_count")]
        public uint SendersCount { get; set; }

        /// <summary>Number of receivers</summary>
        [JsonPropertyName("receivers_count")]
        public uint ReceiversCount { get; set; }

        /// <summary>Number of changes</summary>
        [JsonPropertyName("changesinfo_count")]
        public uint ChangesInfoCount { get; set; }

        /// <summary>Integer with info about how many accounts are signed.Does not check if signature is valid for a multioperation not included in blockchain</summary>
        [JsonPropertyName("signed_count")]
        public uint SignedCount { get; set; }

        /// <summary>Integer with info about how many accounts are pending to be signed</summary>
        [JsonPropertyName("not_signed_count")]
        public uint NotSignedCount { get; set; }

        /// <summary>True if everybody signed.Does not check if MultiOperation is well formed or can be added to Network because is an offline call</summary>
        [JsonPropertyName("signed_can_execute")]
        public bool CanExecute { get; set; }

        
    }
}
