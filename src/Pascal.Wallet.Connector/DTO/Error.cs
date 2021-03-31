// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>JSON-RPC Error DTO <see href="https://www.pascalcoin.org/development/rpc#error-codes">https://www.pascalcoin.org/development/rpc#error-codes</see></summary>
    public class Error
    {
        [JsonPropertyName("code")]
        public ErrorCode Code { get; set; }

        /// <summary>Human readable error description</summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        public Error() { }
        public Error(string message, ErrorCode code)
        {
            Message = message;
            Code = code;
        }
    }
}
