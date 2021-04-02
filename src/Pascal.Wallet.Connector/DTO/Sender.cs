// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    public class SenderBase
    {
        /// <summary>Sending Account</summary>
        [JsonPropertyName("account")]
        public uint AccountNumber { get; set; }

        /// <summary>PASCURRENCY - Amount of coins transferred from sender account to destination account (including transaction fee). When creating MultiOperation, it should be in positive format.</summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>HEXASTRING</summary>
        [JsonPropertyName("payload")]
        public string Payload { get; set; }

        /// <summary>The PayloadType describes the encryption and encoding of the Payload.</summary>
        [JsonPropertyName("payload_type")]
        public PayloadType PayloadType { get; set; }
    }

    /// <summary>Sender structure for creating MultiOperation</summary>
    /// <remarks>For MultiOperations Amount should be set is in positive format</remarks>
    public class Sender: SenderBase
    {        
        /// <summary>If not provided, will use current safebox n_operation+1 value (on online wallets)</summary>
        [JsonPropertyName("n_operation")]
        public uint? NOperation { get; set; }

        //Do not remove this constructor, it is used by deserializer, otherwise deserializer uses parametrized constructor and hexastring encoding is executed twice
        public Sender() { }

        /// <summary>Creates Sender object</summary>
        /// <param name="amount">PASCURRENCY in positive format</param>
        /// <param name="payload">HEXASTRING</param>
        /// <param name="nOperation">If not provided, will use current safebox n_operation+1 value (on online wallets)</param>
        public Sender(uint accountNumber, decimal amount, string payload = null, PayloadType payloadType = PayloadType.NonDeterministic, uint? nOperation = null)
        {
            AccountNumber = accountNumber;
            Amount = amount;
            Payload = payload.ToHexastring();
            PayloadType = payloadType;
            NOperation = nOperation;
        }
    }

    /// <summary>Sender structure returned by Wallet</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#operation-object">https://www.pascalcoin.org/development/rpc#operation-object</see></remarks>
    public class ExtendedSender: SenderBase
    {
        /// <summary>Operations made by Account</summary>
        [JsonPropertyName("n_operation")]
        public uint NOperation { get; set; }

        [JsonPropertyName("account_epasa")]
        public string AccountEpasa { get; set; }

        [JsonPropertyName("unenc_payload")]
        public string UnencryptedPayload { get; set; }

        [JsonPropertyName("unenc_hexpayload")]
        public string UnencryptedPayloadHexastring { get; set; }

        /// <summary></summary>
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }
}
