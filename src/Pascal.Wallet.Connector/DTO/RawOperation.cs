// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc and PIP-0027 https://www.pascalcoin.org/development/pips/pip-0027

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>A "Raw operations object" contains information about a signed operation made by "signsendto" or "signchangekey"</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#raw-operations-object">https://www.pascalcoin.org/development/rpc#raw-operations-object</see></remarks>
    public class RawOperation
    {
        /// <summary>Count how many operations has rawoperations param</summary>
        [JsonPropertyName("operations")]
        public uint OperationsCount { get; set; }

        /// <summary>PASCURRENCY - Total amount</summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>PASCURRENCY - Fee of this operation</summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }

        /// <summary>HEXASTRING - This is the operations in raw format</summary>
        [JsonPropertyName("rawoperations")]
        public string RawOperations { get; set; }
    }
}
