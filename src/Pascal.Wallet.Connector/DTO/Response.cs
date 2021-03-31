// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>JSON RPC Standard 2.0 response DTO</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#json-rpc-standard-20">https://www.pascalcoin.org/development/rpc#json-rpc-standard-20</see></remarks>
    public class Response<T>
    {
        [JsonPropertyName("id")]
        public uint Id { get; set; }

        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonPropertyName("result")]
        public T Result { get; set; }

        /// <summary>If not null then the operation was not successfull and this property contains the Error description</summary>
        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }
}
