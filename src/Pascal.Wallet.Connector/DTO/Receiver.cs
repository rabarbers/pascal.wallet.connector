// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Changer structure for creating MultiOperation</summary>
    public class Receiver
    {
        /// <summary>Receiving Account</summary>
        [JsonPropertyName("account")]
        public uint AccountNumber { get; set; }

        /// <summary>PASCURRENCY - Amount of coins received from sender account.</summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>HEXASTRING</summary>
        [JsonPropertyName("payload")]
        public string Payload { get; set; }

        /// <summary>The PayloadType describes the encryption and encoding of the Payload.</summary>
        [JsonPropertyName("payload_type")]
        public PayloadType PayloadType { get; set; }

        //Do not remove this constructor, it is used be deserializer, otherwise deserialized uses parametrized constructor and hexastring encoding is executed twice
        public Receiver() { }
        
        /// <summary>Cretaes Receiver object</summary>
        /// <param name="payload">HEXASTRING</param>
        public Receiver(uint accountNumber, decimal amount, string payload = null, PayloadType payloadType = PayloadType.NonDeterministic)
        {
            AccountNumber = accountNumber;
            Amount = amount;
            Payload = payload.ToHexastring();
            PayloadType = payloadType;
        }
    }

    /// <summary></summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#operation-object">https://www.pascalcoin.org/development/rpc#operation-object</see></remarks>
    public class ExtendedReceiver: Receiver
    {
        [JsonPropertyName("account_epasa")]
        public string AccountEpasa { get; set; }

        [JsonPropertyName("unenc_payload")]
        public string UnencryptedPayload { get; set; }

        [JsonPropertyName("unenc_hexpayload")]
        public string UnencryptedPayloadHexastring { get; set; }
    }
}
