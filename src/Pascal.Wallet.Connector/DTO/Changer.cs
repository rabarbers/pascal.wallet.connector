// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    public class ChangerBase
    {
        /// <summary>Changing Account</summary>
        [JsonPropertyName("account")]
        public uint AccountNumber { get; set; }

        /// <summary>If provided will update Public key of "account" when the operation is executed</summary>
        [JsonPropertyName("new_enc_pubkey")]
        public string NewEncodedPublicKey { get; set; }

        /// <summary>If provided will change account name when the operation is executed</summary>
        [JsonPropertyName("new_name")]
        public string NewName { get; set; }

        /// <summary>If provided will change account type when the operation is executed</summary>
        [JsonPropertyName("new_type")]
        public int? NewType { get; set; }
    }

    /// <summary>Changer structure for creating MultiOperation</summary>
    public class Changer: ChangerBase
    {
        /// <summary>(optional) - if not provided, will use current safebox n_operation+1 value (on online wallets)</summary>
        [JsonPropertyName("n_operation")]
        public uint? NOperation { get; set; }

        //Do not remove this constructor, it is used be deserializer
        public Changer() { }

        /// <summary></summary>
        /// <param name="accountNumber"></param>
        /// <param name="newEncodedPublicKey"></param>
        /// <param name="newName"></param>
        /// <param name="newType"></param>
        /// <param name="nOperation"></param>
        public Changer(uint accountNumber, string newEncodedPublicKey = null, string newName = null, int? newType = null, uint? nOperation = null)
        {
            AccountNumber = accountNumber;
            NewEncodedPublicKey = newEncodedPublicKey;
            NewName = newName;
            NewType = newType;
            NOperation = nOperation;
        }
    }

    public class ExtendedChanger: ChangerBase
    {
        /// <summary>Operations made by Account</summary>
        [JsonPropertyName("n_operation")]
        public uint NOperation { get; set; }

        /// <summary>If is listed for sale(public or private) will show seller account</summary>
        [JsonPropertyName("seller_account")]
        public uint? SellerAccount { get; set; }

        /// <summary>If is listed for sale (public or private) will show account price</summary>
        [JsonPropertyName("account_price")]
        public decimal AccountPrice { get; set; }

        /// <summary>If is listed for private sale will show block locked</summary>
        [JsonPropertyName("locked_until_block")]
        public uint? LockedUntilBlock { get; set; }

        /// <summary>In negative value, due it's outgoing from "account"</summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }

        /// <summary>Change type</summary>
        [JsonPropertyName("changes")]
        public string Changes { get; set; }
    }
}
