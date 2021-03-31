// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Contains the information about an account</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#account-object">https://www.pascalcoin.org/development/rpc#account-object</see></remarks>
    public class Account
    {
        [JsonPropertyName("account")]
        public uint AccountNumber { get; set; }

        /// <summary> Encoded public key value(See decodepubkey)</summary>
        [JsonPropertyName("enc_pubkey")]
        public string EncodedPublicKey { get; set; }

        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        /// <summary>Operations made by this account</summary>
        [JsonPropertyName("n_operation")]
        public uint NOperation { get; set; }

        /// <summary>Last block that updated this account. If equal to blockchain blocks count it means that it has pending operations to be included to the blockchain.</summary>
        [JsonPropertyName("updated_b")]
        public uint LastUpdatedBlock { get; set; }

        //TODO: find explanation
        [JsonPropertyName("updated_b_active_mode")]
        public uint UpdatedBlockActiveMode { get; set; }

        //TODO: find explanation
        [JsonPropertyName("updated_b_passive_mode")]
        public uint UpdatedBlockPassiveMode { get; set; }

        /// <summary>Values can be normal or listed. When listed then account is for sale</summary>
        [JsonPropertyName("state")]
        public AccountState State { get; set; }

        /// <summary>Until what block this account is locked. Only set if state is listed</summary>
        [JsonPropertyName("locked_until_block")]
        public uint? LockedUntilBlock { get; set; }

        /// <summary>Price of account. Only set if state is listed</summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        /// <summary>Seller's account number. Only set if state is listed</summary>
        [JsonPropertyName("seller_account")]
        public uint? SellerAccount { get; set; }

        /// <summary>For Listed accounts, this indicates whether it's private or public sale</summary>
        [JsonPropertyName("private_sale")]
        public bool PrivateSale { get; set; }

        /// <summary>For Listed accounts for PrivateSale, this indicates the buyers public key</summary>
        [JsonPropertyName("new_enc_pubkey")]
        public string NewPublicKey { get; set; }

        /// <summary>Public name of account. Follows PascalCoin64 Encoding <see href="https://www.pascalcoin.org/development/pips/pip-0004#pascalcoin64">https://www.pascalcoin.org/development/pips/pip-0004#pascalcoin64</see></summary>
        /// <remarks>First char cannot start with number. Must be empty/null or 3..64 characters in length</remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>Type of account. Valid values range from 0..65535</summary>
        [JsonPropertyName("type")]
        public uint Type { get; set; }

        //TODO: find explanation
        /// <summary>HEXASTRING: Hexadecimal value 0..32 bytes</summary>
        [JsonPropertyName("data")]
        public string Data { get; set; }

        //TODO: find explanation
        [JsonPropertyName("seal")]
        public string Seal { get; set; }
    }
}
