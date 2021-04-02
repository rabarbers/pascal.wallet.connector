// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc and PIP-0027 https://www.pascalcoin.org/development/pips/pip-0027

using System;
using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Contains information about an operation</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#operation-object">https://www.pascalcoin.org/development/rpc#operation-object</see></remarks>
    public class Operation
    {
        /// <summary>Block number</summary>
        /// <remarks>Only when valid</remarks>
        [JsonPropertyName("block")]
        public uint? BlockNumber { get; set; }

        /// <summary>Block creation UTC time</summary>
        [JsonConverter(typeof(UnixTimeConverter))]
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        /// <summary>Operation index inside a block (0..operations-1).</summary>
        /// <remarks>If opblock=-1 means that is a blockchain reward (only when valid)</remarks>
        [JsonPropertyName("opblock")]
        public int Index { get; set; }

        /// <summary>Return null when operation is not included on a blockchain yet, 0 means that is included in highest block and so on...</summary>
        [JsonPropertyName("maturation")]
        public uint? Maturation { get; set; }

        /// <summary>Operation type</summary>
        [JsonPropertyName("optype")]
        public OperationType Type { get; set; }

        /// <summary>Associated with operation type, can be used to discriminate from the point of view of operation (sender/receiver/buyer/seller ...)</summary>
        [JsonPropertyName("subtype")]
        public OperationSubType SubType { get; set; }

        /// <summary>Account affected by this operation.</summary>
        /// <remarks>A transaction can affect 2 accounts.</remarks>
        [JsonPropertyName("account")]
        public uint AccountNumber { get; set; }

        /// <summary>Will return the account that signed (and payed fee) for this operation. Not used when is a Multioperation (operation Type = Multioperation)</summary>
        [JsonPropertyName("signer_account")]
        public uint SignerAccountNumber { get; set; }

        /// <summary>Number of operations made by Account</summary>
        [JsonPropertyName("n_operation")]
        public uint NOperation { get; set; }

        /// <summary>Array of objects with senders, for example in a transaction (operation Type = Transaction) or multioperation senders (operation Type = Multioperation)</summary>
        [JsonPropertyName("senders")]
        public ExtendedSender[] Senders { get; set; }

        /// <summary>Array of objects - When is a transaction or multioperation, this array contains each receiver</summary>
        [JsonPropertyName("receivers")]
        public ExtendedReceiver[] Receivers { get; set; }

        /// <summary>Array of objects - When accounts changed state </summary>
        [JsonPropertyName("changers")]
        public ExtendedChanger[] Changers { get; set; }

        /// <summary>Human readable operation type</summary>
        [JsonPropertyName("optxt")]
        public string Description { get; set; }

        /// <summary>PASCURRENCY - Fee of this operation</summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }

        /// <summary>PASCURRENCY - Amount of coins transferred from sender account to destination account including transaction fee (Only apply when operation Type = (Transaction or DataOperation))</summary>
        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        /// <summary></summary>
        [JsonPropertyName("payload")]
        public string Payload { get; set; }

        /// <summary>The PayloadType describes the encryption and encoding of the Payload.</summary>
        [JsonPropertyName("payload_type")]
        public PayloadType PayloadType { get; set; }

        /// <summary>HEXASTRING - Operation hash used to find this operation in the blockchain</summary>
        [JsonPropertyName("ophash")]
        public string OpHash { get; set; }

        /// <summary>PASCURRENCY - Balance of account after this block is introduced in the Blockchain</summary>
        /// <remarks>Balance is a calculation based on current safebox account balance and previous operations, it's only returned on pending operations and account operations.</remarks>
        [JsonPropertyName("balance")]
        public decimal? Balance { get; set; }
    }

    /// <summary>Contains information about an operation created offline</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#operation-object">https://www.pascalcoin.org/development/rpc#operation-object</see></remarks>
    public class ColdWalletOperation: Operation
    {
        /// <summary>Block number</summary>
        /// <remarks>Boolean(optional) - If operation is invalid, value=false</remarks>
        [JsonPropertyName("valid")]
        public bool Valid { get; set; } = true;

        /// <summary>String (optional) - If operation is invalid, an error description</summary>
        [JsonPropertyName("errors")]
        public string Errors { get; set; }
    }
}
