// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

using Pascal.Wallet.Connector.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Pascal.Wallet.Connector
{
    public sealed class PascalConnector : IDisposable
    {
        private const string defaultAddress = "127.0.0.1";
        private const uint defaultPort = 4003;
        private readonly Uri _url;
        private readonly HttpClient _httpClient;
        private uint _callId;

        public PascalConnector(string address = defaultAddress, uint port = defaultPort) : this(new Uri($"http://{address}:{port}"), new HttpClient()) { }

        public PascalConnector(Uri url) : this(url, new HttpClient()) { }

        public PascalConnector(Uri url, HttpClient client)
        {
            _url = url;
            _httpClient = client;
        }

        /// <summary>Adds a node to connect</summary>
        /// <param name="nodes">String containing 1 or multiple IP:port separated by ";"</param>
        /// <returns>Returns an integer with nodes added</returns>
        public Task<Response<int?>> AddNodeAsync(string nodes)
        {
            return InvokeAsync<int?>("addnode", $"{{\"nodes\":\"{nodes}\"}}");
        }

        /// <summary>Get an account information</summary>
        /// <param name="account">Cardinal containing account number</param>
        public Task<Response<Account>> GetAccountAsync(uint account)
        {
            return InvokeAsync<Account>("getaccount", $"{{\"account\":{account}}}");
        }

        /// <summary>Get available wallet accounts information(all or filtered by public key)</summary>
        /// <param name="encodedPublicKey">If provided, return only accounts of this public key</param>
        /// <param name="b58PublicKey">If provided, return only accounts of this public key </param>
        /// <param name="start">If provided, will return wallet accounts starting at this position (index starts at position 0). If not provided, start=0 by default.</param>
        /// <param name="max">If provided, will return max accounts. If not provided, max=100 by default.</param>
        /// <remarks>If use encodedPublicKey and b58PublicKey together and is not the same public key, will return an error</remarks>
        public Task<Response<Account[]>> GetWalletAccountsAsync(string encodedPublicKey = null, string b58PublicKey = null, uint? start = null, uint? max = null)
        {
            var parameters = new
            {
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey,
                start,
                max
            };
            return InvokeAsync<Account[]>("getwalletaccounts", Serialize(parameters));
        }

        /// <summary>Get number of available wallet accounts(total or filtered by public key)</summary>
        /// <param name="encodedPublicKey">If provided, return only accounts of this public key</param>
        /// <param name="b58PublicKey">If provided, return only accounts of this public key </param>
        /// <param name="start">If provided, will return wallet accounts starting at this position (index starts at position 0). If not provided, start=0 by default.</param>
        /// <param name="max">If provided, will return max accounts. If not provided, max=100 by default.</param>
        /// <remarks>If use encodedPublicKey and b58PublicKey together and is not the same public key, will return an error</remarks>
        public Task<Response<uint?>> GetWalletAccountsCountAsync(string encodedPublicKey = null, string b58PublicKey = null, uint? start = null, uint? max = null)
        {
            var parameters = new
            {
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey,
                start,
                max
            };
            return InvokeAsync<uint?>("getwalletaccountscount", Serialize(parameters));
        }

        /// <summary>Get wallet public keys</summary>
        /// <param name="start">If provided, will return wallet public keys starting at this position (index starts at position 0). If not provided, start=0 by default.</param>
        /// <param name="max">If provided, will return max public keys. If not provided, max=100 by default.</param>
        public Task<Response<WalletPublicKey[]>> GetWalletPublicKeysAsync(uint? start = null, uint? max = null)
        {
            var parameters = new
            {
                start,
                max
            };
            return InvokeAsync<WalletPublicKey[]>("getwalletpubkeys", Serialize(parameters));
        }

        /// <summary>Search for a public key in the wallet. If use encodedPublicKey and b58PublicKey together and is not the same public key, will return an error.</summary>
        /// <param name="encodedPublicKey">HEXASTRING (optional). If provided, return only this public key balance</param>
        /// <param name="b58PublicKey">String (optional). If provided, return only this public key balance</param>
        public Task<Response<PublicKey>> GetWalletPublicKeyAsync(string encodedPublicKey = null, string b58PublicKey = null)
        {
            var parameters = new
            {
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey
            };
            return InvokeAsync<PublicKey>("getwalletpubkey", Serialize(parameters));
        }

        /// <summary>Get wallet coins total balance (total or filtered by public key). If use encodedPublicKey and b58PublicKey together and is not the same public key, will return an error.</summary>
        /// <param name="encodedPublicKey">HEXASTRING (optional). If provided, return only this public key balance</param>
        /// <param name="b58PublicKey">String (optional). If provided, return only this public key balance</param>
        public Task<Response<double?>> GetWalletCoinsAsync(string encodedPublicKey = null, string b58PublicKey = null)
        {
            var parameters = new
            {
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey
            };
            return InvokeAsync<double?>("getwalletcoins", Serialize(parameters));
        }

        /// <summary>Get block information</summary>
        /// <param name="blockNumber">Block number (0..blocks count-1)</param>
        public Task<Response<Block>> GetBlockAsync(uint blockNumber)
        {
            return InvokeAsync<Block>("getblock", $"{{\"block\":{blockNumber}}}");
        }

        /// <summary>Get a list of last n blocks</summary>
        /// <param name="lastNBlocks">Last n blocks in the blockchain</param>
        public Task<Response<Block[]>> GetBlocksAsync(uint lastNBlocks)
        {
            return InvokeAsync<Block[]>("getblocks", $"{{\"last\":{lastNBlocks}}}");
        }

        /// <summary>Get a list of blocks from start to end</summary>
        public Task<Response<Block[]>> GetBlocksAsync(uint start, uint end)
        {
            return InvokeAsync<Block[]>("getblocks", $"{{\"start\":{start}, \"end\":{end}}}");
        }

        /// <summary>Get blockchain high in this node</summary>
        public Task<Response<uint?>> GetBlockCountAsync()
        {
            return InvokeAsync<uint?>("getblockcount");
        }

        /// <summary>Get an operation of the block information</summary>
        /// <param name="blockNumber">Block number</param>
        /// <param name="operationNumber">Operation(0..operations-1) of this block</param>
        public Task<Response<Operation>> GetBlockOperationAsync(uint blockNumber, uint operationNumber)
        {
            return InvokeAsync<Operation>("getblockoperation", $"{{\"block\":{blockNumber},\"opblock\":{operationNumber}}}");
        }

        /// <summary>Get all operations of specified block</summary>
        /// <param name="block">Block number</param>
        /// <param name="start">If provided, will start at this position(index starts at position 0). If not provided, start=0 by default.</param>
        /// <param name="max">If provided, will return max registers. If not provided, max=100 by default.</param>
        public Task<Response<Operation[]>> GetBlockOperationsAsync(uint block, uint? start = null, uint? max = null)
        {
            var parameters = new
            {
                block,
                start,
                max
            };
            return InvokeAsync<Operation[]>("getblockoperations", Serialize(parameters));
        }

        /// <summary>Get operations made to an account</summary>
        /// <param name="account">Account number(0..accounts count-1)</param>
        /// <param name="depth">(Optional, default value 100) Depth to search on blocks where this account has been affected.Allowed to use deep as a param name too.</param>
        /// <param name="start">(optional, default = 0). If provided, will start at this position(index starts at position 0). If start is -1, then will include pending operations, otherwise only operations included on the blockchain</param>
        /// <param name="max">Integer(optional, default = 100). If provided, will return max registers.If not provided, max= 100 by default</param>
        public Task<Response<Operation[]>> GetAccountOperationsAsync(uint account, uint? depth = null, int? start = null, uint? max = null)
        {
            var parameters = new
            {
                account,
                depth,
                start,
                max
            };
            return InvokeAsync<Operation[]>("getaccountoperations", Serialize(parameters));
        }

        /// <summary>Get pending operations made to an account</summary>
        /// <param name="start">Default value 0</param>
        /// <param name="max">Default value 100, use 0=All</param>
        public Task<Response<Operation[]>> GetPendingsAsync(int? start = null, uint? max = null)
        {
            var parameters = new
            {
                start,
                max
            };
            return InvokeAsync<Operation[]>("getpendings", Serialize(parameters));
        }

        /// <summary>Returns node pending buffer count</summary>
        public Task<Response<uint?>> GetPendingsCountAsync()
        {
            return InvokeAsync<uint?>("getpendingscount");
        }

        /// <summary>Finds an operation by "ophash"</summary>
        /// <param name="ophash">Value ophash received on an operation</param>
        public Task<Response<Operation>> FindOperationAsync(string ophash)
        {
            return InvokeAsync<Operation>("findoperation", $"{{\"ophash\":\"{ophash}\"}}");
        }

        /// <summary>Find accounts by name/type</summary>
        /// <param name="name">If has value, will return the account that match name</param>
        /// <param name="type">If has value, will return accounts with same type</param>
        /// <param name="start">Start account(by default, 0) - NOTE: Is the "start account number", when executing multiple calls you must set start value to the latest returned account number + 1 (Except if searching by public key, see below)</param>
        /// <param name="max">Max of accounts returned in array(by default, 100)</param>
        /// <param name="exact">(True by default) - If False and name has value will return accounts containing name value in it's name (multiple accounts can match)</param>
        /// <param name="minBalance">PASCURRENCY - If have value, will filter by current account balance</param>
        /// <param name="maxBalance">PASCURRENCY - If have value, will filter by current account balance</param>
        /// <param name="encodedPublicKey">HEXASTRING - Will return accounts with this public key. NOTE: When searching by public key the start param value is the position of indexed public keys list instead of accounts numbers</param>
        /// <param name="b58PublicKey">String - Will return accounts with this public key. NOTE: When searching by public key the start param value is the position of indexed public keys list instead of accounts numbers</param>
        public Task<Response<Account[]>> FindAccountsAsync(string name = null, uint? type = null, uint? start = null, uint? max = null, bool? exact = null, decimal? minBalance = null, decimal? maxBalance = null, string encodedPublicKey = null, string b58PublicKey = null)
        {
            var parameters = new
            {
                name,
                type,
                start,
                max,
                exact,
                min_balance = minBalance,
                max_balance = maxBalance,
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey
            };
            return InvokeAsync<Account[]>("findaccounts", Serialize(parameters));
        }

        /// <summary>Executes a transaction operation from "senderAccount" to "receiverAccount"</summary>
        /// <param name="senderAccount">Sender account</param>
        /// <param name="receiverAccount">Destination account</param>
        /// <param name="amount">Coins to be transferred</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = Destination)</param>
        /// <param name="password">Used to encrypt payload with aes as a payload_method. If none equals to empty password.</param>
        /// <returns>Operation object</returns>
        public Task<Response<Operation>> SendToAsync(uint senderAccount, uint receiverAccount, decimal amount, decimal? fee = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                sender = senderAccount,
                target = receiverAccount,
                amount,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation>("sendto", Serialize(parameters));
        }

        /// <summary>Sends data to another account</summary>
        /// <param name="senderAccount">Sender account number</param>
        /// <param name="receiverAccount">Destination account number</param>
        /// <param name="signerAccount">Account number of the signer which pays the fee</param>
        /// <param name="guid">GUID created by the sender</param>
        /// <param name="dataType">Type of the message (by default = 0 - Chat message))</param>
        /// <param name="dataSequence">Sequence is for chaining multiple data packets together into a logical blob (by default = 0)</param>
        /// <param name="amount">PASC quantity to transfer (by default = 0)</param>
        /// <param name="fee">Fee of the operation (by default = 0)</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = None)</param>
        /// <param name="password">Used to encrypt payload with aes as a payload_method. If none equals to empty password.</param>
        /// <returns>Operation object</returns>
        public Task<Response<Operation>> SendDataAsync(uint senderAccount, uint receiverAccount, string guid = null, uint? signerAccount = null, DataType? dataType = null, uint? dataSequence = null, decimal? amount = null, decimal? fee = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                sender = senderAccount,
                target = receiverAccount,
                guid = !string.IsNullOrEmpty(guid) ? $"{{{guid}}}" : null,
                signer = signerAccount,
                data_type = (int?)dataType,
                data_sequence = dataSequence,
                amount,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation>("senddata", Serialize(parameters));
        }

        /// <summary>Searches for DataOperations in the blockchain</summary>
        /// <param name="senderAccount">Sender account number (sender or destination is required)</param>
        /// <param name="receiverAccount">Destination account number (sender or destination is required)</param>
        /// <param name="guid">GUID(optional)</param>
        /// <param name="dataType">Integer(optional) Type of the message.</param>
        /// <param name="dataSequence">Integer(optional) Sequence is for chaining multiple data packets together into a logical blob</param>
        /// <param name="start">Integer(optional, default = 0)</param>
        /// <param name="max">Integer(optional, default = 100) Limits the number of items in the output.</param>
        public Task<Response<Operation[]>> FindDataOperationsAsync(uint? senderAccount = null, uint? receiverAccount = null, string guid = null, DataType? dataType = null, uint? dataSequence = null, int? start = null, uint? max = null)
        {
            var parameters = new
            {
                sender = senderAccount,
                target = receiverAccount,
                guid = !string.IsNullOrEmpty(guid) ? $"{{{guid}}}" : null,
                data_type = (int?)dataType,
                data_sequence = dataSequence,
                start,
                max
            };
            return InvokeAsync<Operation[]>("finddataoperations", Serialize(parameters));
        }

        /// <summary>Executes a change key operation, changing "account" public key for a new one. Note that new one public key can be another Wallet public key, or none. When none, it's like a transaction, tranferring account owner to an external owner.</summary>
        /// <param name="account">Integer - Account number to change key</param>
        /// <param name="newEncodedPublicKey">New public key in encoded format</param>
        /// <param name="newB58PublicKey">String - New public key in Base 58 format (the same that Application Wallet exports)</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>Operation object</returns>
        public Task<Response<Operation>> ChangeKeyAsync(uint account, string newEncodedPublicKey = null, string newB58PublicKey = null, decimal? fee = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation>("changekey", Serialize(parameters));
        }

        /// <summary>Executes a change key over multiple accounts.</summary>
        /// <param name="accounts">List of accounts</param>
        /// <param name="newEncodedPublicKey">New public key in encoded format</param>
        /// <param name="newB58PublicKey">String - New public key in Base 58 format (the same that Application Wallet exports)</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>Array of Operation objects</returns>
        public Task<Response<Operation[]>> ChangeKeysAsync(IEnumerable<uint> accounts, string newEncodedPublicKey = null, string newB58PublicKey = null, decimal? fee = null, string payload = null, PayloadMethod? payloadMethod = PayloadMethod.None, string password = null)
        {
            var parameters = new
            {
                accounts = PascalConnectorHelper.AccountsToString(accounts),
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation[]>("changekeys", Serialize(parameters));
        }

        /// <summary>Lists an account for sale (public or private).</summary>
        /// <param name="accountForSale">Account to be listed</param>
        /// <param name="sellerAccount">Account that will receive price amount on sell</param>
        /// <param name="price">Price account can be purchased for</param>
        /// <param name="signerAccount">Account that signs and pays the fee(must have same public key that listed account, or be the same)</param>
        /// <param name="fee">PASCURRENCY - Fee of the operation</param>
        /// <param name="newEncodedPublicKey">If used, then will be a private sale</param>
        /// <param name="newB58PublicKey">If used, then will be a private sale</param>
        /// <param name="lockedUntilBlock">Block number until this account will be locked (a locked account cannot execute operations while locked)</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload</param>
        /// <param name="password">Used to encrypt payload with Aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>If operation is successfull will return an Operation object.</returns>
        public Task<Response<Operation>> ListAccountForSaleAsync(uint accountForSale, uint sellerAccount, decimal price, uint signerAccount, decimal? fee = null, string newEncodedPublicKey = null,
            string newB58PublicKey = null, uint lockedUntilBlock = 0, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account_target = accountForSale,
                seller_account = sellerAccount,
                price,
                account_signer = signerAccount,
                fee,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                locked_until_block = lockedUntilBlock,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation>("listaccountforsale", Serialize(parameters));
        }

        /// <summary>Delist an account for sale</summary>
        /// <param name="accountNumber">Account to be delisted</param>
        /// <param name="signerAccount">Account that signs and pays the fee(must have same public key that listed account, or be the same)</param>
        /// <param name="sellerAccount">Account that will receive price amount on sell</param>
        /// <param name="fee">PASCURRENCY - Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload</param>
        /// <param name="password">Used to encrypt payload with Aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>If operation is successfull will return an Operation object.</returns>
        public Task<Response<Operation>> DelistAccountForSaleAsync(uint accountNumber, uint signerAccount, decimal? fee = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account_target = accountNumber,
                account_signer = signerAccount,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation>("delistaccountforsale", Serialize(parameters));
        }

        /// <summary>Buy an account previously listed for sale(public or private)</summary>
        /// <param name="buyerAccount">Account number of buyer who is purchasing the account</param>
        /// <param name="accountToPurchase">Account number being purchased</param>
        /// <param name="price">Settlement price of account being purchased</param>
        /// <param name="sellerAccount">Account of seller, receiving payment</param>
        /// <param name="newEncodedPublicKey">Post-settlement public key in encoded format. Supply only one value: newEncodedPublicKey or newB58PublicKey.</param>
        /// <param name="newB58PublicKey">Post-settlement public key in base58 format. Supply only one value: newEncodedPublicKey or newB58PublicKey.</param>
        /// <param name="amount">Amount being transferred from buyer_account to seller_account (the settlement). This is a PASCURRENCY value.</param>
        /// <param name="fee">PASCURRENCY - Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload</param>
        /// <param name="password">Used to encrypt payload with Aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>If operation is successfull will return an Operation object.</returns>
        public Task<Response<Operation>> BuyAccountAsync(uint buyerAccount, uint accountToPurchase, decimal amount, decimal? fee = null, string newEncodedPublicKey = null,
            string newB58PublicKey = null, decimal? price = null, uint? sellerAccount = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                buyer_account = buyerAccount,
                account_to_purchase = accountToPurchase,
                price,
                seller_account = sellerAccount,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                amount,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation>("buyaccount", Serialize(parameters));
        }

        /// <summary>Changes an account Public key, or name, or type value</summary>
        /// <param name="accountTarget">Target account</param>
        /// <param name="signerAccount">Account that signs and pays the fee (must have same public key that target account, or be the same)</param>
        /// <param name="newEncodedPublicKey">HEXASTRING. If used, then will change the target account public key</param>
        /// <param name="newB58PublicKey">HEXASTRING. If used, then will change the target account public key</param>
        /// <param name="newName">If used, then will change the target account name</param>
        /// <param name="newType">If used, then will change the target account type</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = Destination)</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>Operation object</returns>
        public Task<Response<Operation>> ChangeAccountInfoAsync(uint accountTarget, uint? signerAccount = null, string newEncodedPublicKey = null,
            string newB58PublicKey = null, string newName = null, uint? newType = null, decimal? fee = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account_target = accountTarget,
                account_signer = signerAccount,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                new_name = newName,
                new_type = newType,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<Operation>("changeaccountinfo", Serialize(parameters));
        }

        /// <summary>Creates and signs a transaction, but does not transfer it to network</summary>
        /// <param name="senderAccount">Sender account number</param>
        /// <param name="receiverAccount">Target account number</param>
        /// <param name="senderEncodedPublicKey">HEXASTRING - Public key of the sender account in encoded format</param>
        /// <param name="senderB58PublicKey">HEXASTRING - Public key of the sender account in b58 format</param>
        /// <param name="targetEncodedPublicKey">HEXASTRING - Public key of the target account in encoded format</param>
        /// <param name="targetB58PublicKey">HEXASTRING - Public key of the target account in b58 format</param>
        /// <param name="rawOperations">HEXASTRING(optional) - If we want to add a sign operation with other previous operations, here we must put previous rawoperations result</param>
        /// <param name="lastNOperation">Last value of n_operation obtained with an Account object, for example when called to getaccount</param>
        /// <param name="amount">Coins to be transferred</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = Destination)</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <remarks>Wallet must be unlocked and sender private key(searched with provided public key) must be in wallet. No other checks are made(no checks for valid target, valid n_operation, valid amount or fee ...)</remarks>
        /// <returns>Returns a Raw Operations Object</returns>
        public Task<Response<RawOperation>> SignSendToAsync(uint senderAccount, uint receiverAccount, uint amount, uint lastNOperation, decimal fee = 0, string senderEncodedPublicKey = null, string senderB58PublicKey = null,
            string targetEncodedPublicKey = null, string targetB58PublicKey = null, string rawOperations = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                sender = senderAccount,
                target = receiverAccount,
                sender_enc_pubkey = senderEncodedPublicKey,
                sender_b58_pubkey = senderB58PublicKey,
                target_enc_pubkey = targetEncodedPublicKey,
                target_b58_pubkey = targetB58PublicKey,
                rawoperations = rawOperations,
                last_n_operation = lastNOperation,
                amount,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<RawOperation>("signsendto", Serialize(parameters));
        }

        /// <summary>Creates and signs a "DATA" operation for later use</summary>
        /// <param name="signerAccount">The signer of the operation</param>
        /// <param name="senderAccount">The sender of the operation. If the sender is null, the signer will be used (default = signer)</param>
        /// <param name="receiverAccount">The account where the DATA operation is send to. If the target is null, the sender or (if null) the signer is used (default = sender or signer)</param>
        /// <param name="guid">A 16 Bytes GUID in 8-4-4-4-12 format. If null or not given, the node will generate a UUID V4 (random)</param>
        /// <param name="lastNOperation">Last value of n_operation of the signerAccount (or senderAccount or receiverAccount)</param>
        /// <param name="dataType">The data type of the operation. If not given, the dataType will be 0 - ChatMessage</param>
        /// <param name="dataSequence">The data sequence of the operation. If the data sequence cannot be determined (null or not given), it is 0 by default</param>
        /// <param name="amount">The amount to transfer to receiver account, if not given or null the default value is 0</param>
        /// <param name="fee">The fee for the operation, if not given or null the default value is 0</param>
        /// <param name="rawOperations">HEXASTRING(optional) - If we want to add a sign operation with other previous operations, here we must put previous rawoperations result</param>
        /// <param name="signerEncodedPublicKey">HEXASTRING - The current public key of signerAccount in encoded format</param>
        /// <param name="signerB58PublicKey">HEXASTRING - The current public key of signerAccount in b58 format</param>
        /// <param name="receiverEncodedPublicKey">HEXASTRING - The current public key of receiverAccount in encoded format</param>
        /// <param name="receiverB58PublicKey">HEXASTRING - The current public key of receiverAccount in b58 format</param>
        /// <param name="senderEncodedPublicKey">HEXASTRING - The current public key of senderAccount in encoded format</param>
        /// <param name="senderB58PublicKey">HEXASTRING - The current public key of senderAccount in b58 format</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = None)</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>Returns a Raw Operations Object</returns>
        public Task<Response<RawOperation>> SignDataAsync(uint signerAccount, uint? senderAccount = null, uint? receiverAccount = null, string guid = null, uint? lastNOperation = null, DataType? dataType = null,
            uint? dataSequence = null, decimal? amount = null, decimal? fee = null, string rawOperations = null, string signerEncodedPublicKey = null, string signerB58PublicKey = null, string receiverEncodedPublicKey = null,
            string receiverB58PublicKey = null, string senderEncodedPublicKey = null, string senderB58PublicKey = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                signer = signerAccount,
                sender = senderAccount,
                target = receiverAccount,
                guid = !string.IsNullOrEmpty(guid) ? $"{{{guid}}}" : null,
                data_type = (int)dataType,
                data_sequence = dataSequence,
                last_n_operation = lastNOperation,
                amount,
                fee,
                rawoperations = rawOperations,
                signer_enc_pubkey = signerEncodedPublicKey,
                signer_b58_pubkey = signerB58PublicKey,
                target_enc_pubkey = receiverEncodedPublicKey,
                target_b58_pubkey = receiverB58PublicKey,
                sender_enc_pubkey = senderEncodedPublicKey,
                sender_b58_pubkey = senderB58PublicKey,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<RawOperation>("signdata", Serialize(parameters));
        }

        /// <summary>Creates and signs a change key over an account, but does not transfer it to network</summary>
        /// <param name="account">Account number to change key</param>
        /// <param name="oldEncodedPublicKey">HEXASTRING - Public key of the account in encoded format</param>
        /// <param name="oldB58PublicKey">HEXASTRING - Public key of the account in b58 format</param>
        /// <param name="newEncodedPublicKey">HEXASTRING - Public key of the new key for the account in encoded format</param>
        /// <param name="newB58PublicKey">HEXASTRING - Public key of the new key for the account in b58 format</param>
        /// <param name="rawOperations">HEXASTRING(optional) - If we want to add a sign operation with other previous operations, here we must put previous rawoperations result</param>
        /// <param name="lastNOperation">Last value of n_operation obtained with an Account object, for example when called to getaccount</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = Destination)</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <remarks>Wallet must be unlocked and private key (searched with provided public key) must be in wallet. No other checks are made (no checks for valid n_operation, valid fee ...) </remarks>
        /// <returns>Returns a Raw Operations Object</returns>
        public Task<Response<RawOperation>> SignChangeKeyAsync(uint account, uint lastNOperation, decimal fee = 0, string oldEncodedPublicKey = null, string oldB58PublicKey = null,
            string newEncodedPublicKey = null, string newB58PublicKey = null, string rawOperations = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account,
                old_enc_pubkey = oldEncodedPublicKey,
                old_b58_pubkey = oldB58PublicKey,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                rawoperations = rawOperations,
                last_n_operation = lastNOperation,
                fee,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<RawOperation>("signchangekey", Serialize(parameters));
        }

        /// <summary>Signs a List Account For Sale operation useful for offline, cold wallets</summary>
        /// <param name="accountForSale">Account to be listed</param>
        /// <param name="sellerAccount">Account that will receive price amount on sell</param>
        /// <param name="price">Price account can be purchased for/param>
        /// <param name="lastNOperation">Last value of n_operation obtained with an Account object, for example when called to getaccount</param>
        /// <param name="signerAccount">Account that signs and pays the fee(must have same public key that listed account, or be the same)</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="newEncodedPublicKey">HEXASTRING - If used, then will be a private sale</param>
        /// <param name="newB58PublicKey">HEXASTRING - If used, then will be a private sale</param>
        /// <param name="lockedUntilBlock">Block number until this account will be locked (a locked account cannot execute operations while locked)</param>
        /// <param name="rawOperations">HEXASTRING(optional) - If we want to add a sign operation with other previous operations, here we must put previous rawoperations result</param>
        /// <param name="signerEncodedPublicKey">HEXASTRING - The current public key of account_signer in encoded format</param>
        /// <param name="signerB58PublicKey">HEXASTRING - The current public key of account_signer in b58 format</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = Destination)</param>
        /// <param name="password">Used to encrypt payload with aes as a payload_method. If none equals to empty password.</param>
        /// <returns>Returns a Raw Operations Object</returns>
        public Task<Response<RawOperation>> SignListAccountForSaleAsync(uint accountForSale, uint sellerAccount, decimal price, uint lastNOperation, uint signerAccount, decimal fee = 0,
            string newEncodedPublicKey = null, string newB58PublicKey = null, uint lockedUntilBlock = 0, string rawOperations = null, string signerEncodedPublicKey = null, string signerB58PublicKey = null,
            string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account_target = accountForSale,
                seller_account = sellerAccount,
                price,
                last_n_operation = lastNOperation,
                account_signer = signerAccount,
                fee,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                locked_until_block = lockedUntilBlock,
                rawoperations = rawOperations,
                signer_enc_pubkey = signerEncodedPublicKey,
                signer_b58_pubkey = signerB58PublicKey,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<RawOperation>("signlistaccountforsale", Serialize(parameters));
        }

        /// <summary>Delists an Account From Sale operation useful for offline, cold wallets</summary>
        /// <param name="accountNumber">Account to be listed</param>
        /// <param name="signerAccount">Account that signs and pays the fee(must have same public key that listed account, or be the same)</param>
        /// <param name="lastNOperation">Last value of n_operation obtained with an Account object, for example when called to getaccount</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="rawOperations">HEXASTRING(optional) - If we want to add a sign operation with other previous operations, here we must put previous rawoperations result</param>
        /// <param name="signerEncodedPublicKey">HEXASTRING - The current public key of signerAccount in encoded format</param>
        /// <param name="signerB58PublicKey">HEXASTRING - The current public key of signerAccount in b58 format</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = Destination)</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>Returns a Raw Operations Object</returns>
        public Task<Response<RawOperation>> SignDelistAccountForSaleAsync(uint accountNumber, uint signerAccount, uint lastNOperation, decimal fee = 0,
            string rawOperations = null, string signerEncodedPublicKey = null, string signerB58PublicKey = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account_target = accountNumber,
                account_signer = signerAccount,
                last_n_operation = lastNOperation,
                fee,
                rawoperations = rawOperations,
                signer_enc_pubkey = signerEncodedPublicKey,
                signer_b58_pubkey = signerB58PublicKey,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<RawOperation>("signdelistaccountforsale", Serialize(parameters));
        }

        /// <summary>Signs a buy operation for cold wallets</summary>
        /// <param name="buyerAccount">Account number of buyer who is purchasing the account</param>
        /// <param name="accountToPurchase">Account number being purchased</param>
        /// <param name="amount">Amount being transferred from buyer_account to seller_account (the settlement). This is a PASCURRENCY value.</param>
        /// <param name="lastNOperation">The current n_operation of buyer account</param>
        /// <param name="fee">PASCURRENCY - Fee of the operation</param>
        /// <param name="newEncodedPublicKey">Post-settlement public key in encoded format. Supply only one value: newEncodedPublicKey or newB58PublicKey.</param>
        /// <param name="newB58PublicKey">Post-settlement public key in base58 format. Supply only one value: newEncodedPublicKey or newB58PublicKey.</param>
        /// <param name="price">Settlement price of account being purchased</param>
        /// <param name="sellerAccount">Account of seller, receiving payment</param>
        /// <param name="rawOperations">HEXASTRING with previous signed operations (optional)</param>
        /// <param name="signerEncodedPublicKey">HEXASTRING - The current public key of buyer_account in encoded format</param>
        /// <param name="signerB58PublicKey">HEXASTRING - The current public key of buyer_account in b58 format</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload</param>
        /// <param name="password">Used to encrypt payload with Aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>Returns a Raw Operations Object</returns>
        public Task<Response<RawOperation>> SignBuyAccountAsync(uint buyerAccount, uint accountToPurchase, uint sellerAccount, decimal price, decimal amount, uint lastNOperation, decimal? fee = null,
            string newEncodedPublicKey = null, string newB58PublicKey = null, string rawOperations = null, string signerEncodedPublicKey = null, string signerB58PublicKey = null,
            string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                buyer_account = buyerAccount,
                account_to_purchase = accountToPurchase,
                price,
                seller_account = sellerAccount,
                last_n_operation = lastNOperation,
                amount,
                fee,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                rawoperations = rawOperations,
                signer_enc_pubkey = signerEncodedPublicKey,
                signer_b58_pubkey = signerB58PublicKey,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<RawOperation>("signbuyaccount", Serialize(parameters));
        }

        /// <summary>Signs a change account info for cold wallets</summary>
        /// <param name="accountTarget">Target account</param>
        /// <param name="signerAccount">Account that signs and pays the fee (must have same public key that target account, or be the same)</param>
        /// <param name="lastNOperation">The current n_operation of buyer account</param>
        /// <param name="newEncodedPublicKey">HEXASTRING. If used, then will change the target account public key</param>
        /// <param name="newB58PublicKey">HEXASTRING. If used, then will change the target account public key</param>
        /// <param name="newName">If used, then will change the target account name</param>
        /// <param name="newType">If used, then will change the target account type</param>
        /// <param name="fee">Fee of the operation</param>
        /// <param name="rawOperations">HEXASTRING(optional) - If we want to add a sign operation with other previous operations, here we must put previous rawoperations result</param>
        /// <param name="signerEncodedPublicKey">HEXASTRING - The current public key of account_signer in encoded format</param>
        /// <param name="signerB58PublicKey">HEXASTRING - The current public key of account_signer in b58 format</param>
        /// <param name="payload">Payload "item" that will be included in this operation</param>
        /// <param name="payloadMethod">Encode type of the item payload (by default = Destination)</param>
        /// <param name="password">Used to encrypt payload with aes as a payloadMethod. If none equals to empty password.</param>
        /// <returns>Returns a Raw Operations Object</returns>
        public Task<Response<RawOperation>> SignChangeAccountInfoAsync(uint accountTarget, uint signerAccount, uint lastNOperation, string newEncodedPublicKey = null,
            string newB58PublicKey = null, string newName = null, uint? newType = null, decimal? fee = null, string rawOperations = null,
            string signerEncodedPublicKey = null, string signerB58PublicKey = null, string payload = null, PayloadMethod? payloadMethod = null, string password = null)
        {
            var parameters = new
            {
                account_target = accountTarget,
                last_n_operation = lastNOperation,
                account_signer = signerAccount,
                new_enc_pubkey = newEncodedPublicKey,
                new_b58_pubkey = newB58PublicKey,
                new_name = newName,
                new_type = newType,
                fee,
                rawoperations = rawOperations,
                signer_enc_pubkey = signerEncodedPublicKey,
                signer_b58_pubkey = signerB58PublicKey,
                payload = payload?.ToHexString(),
                payload_method = payloadMethod?.ToString().ToLower(),
                pwd = password
            };
            return InvokeAsync<RawOperation>("signchangeaccountinfo", Serialize(parameters));
        }

        /// <summary>Gets information about a signed operation without transfering it to network</summary>
        /// <param name="rawOperations">HEXASTRING</param>
        /// <remarks>Remember that rawoperations are operations that maybe are not correct</remarks>
        /// <returns>Returns a JSON Array with Operation Object items, one for each operation in rawoperations param.</returns>
        public Task<Response<Operation[]>> OperationsInfoAsync(string rawOperations)
        {
            return InvokeAsync<Operation[]>("operationsinfo", $"{{\"rawoperations\":\"{rawOperations}\"}}");
        }

        /// <summary>Executes a signed operations and transfers to the network</summary>
        /// <param name="rawOperations">HEXASTRING</param>
        /// <remarks>For each Operation Object item, if there is an error, param Valid will be false and param Errors will show error description. Otherwise, operation is correct and will contain ophash param</remarks>
        /// <returns>Returns a JSON Array with Operation Object items, one for each operation in rawoperations param</returns>
        public Task<Response<ColdWalletOperation[]>> ExecuteOperationsAsync(string rawOperations)
        {
            return InvokeAsync<ColdWalletOperation[]>("executeoperations", $"{{\"rawoperations\":\"{rawOperations}\"}}");
        }

        /// <summary>Returns information about the Node</summary>
        public Task<Response<NodeStatus>> NodeStatusAsync()
        {
            return InvokeAsync<NodeStatus>("nodestatus");
        }

        /// <summary>Encodes a public key based on params information</summary>
        /// <param name="x">HEXASTRING with x value of public key</param>
        /// <param name="y">HEXASTRING with y value of public key</param>
        /// <returns>Returns a HEXASTRING with encoded public key</returns>
        public Task<Response<string>> EncodePublicKeyAsync(EncryptionType encryptionType, string x, string y)
        {
            return InvokeAsync<string>("encodepubkey", $"{{\"ec_nid\":{(int)encryptionType}, \"x\":\"{x}\", \"y\":\"{y}\"}}");
        }

        /// <summary>Decodes an encoded public key</summary>
        /// <param name="encodedPublicKey">Public key in encoded format</param>
        /// <param name="b58PublicKey">Public key in b58 format</param>
        /// <remarks>Provide encodedPublicKey or b58PublicKey. If encodedPublicKey and b58PublicKey are used together then it should be the same public key.</remarks>
        /// <returns>PublicKey object</returns>
        public Task<Response<PublicKey>> DecodePublicKeyAsync(string encodedPublicKey = null, string b58PublicKey = null)
        {
            var parameters = new
            {
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey
            };
            return InvokeAsync<PublicKey>("decodepubkey", Serialize(parameters));
        }

        /// <summary>Encrypt a text "paylad" using "payload_method"</summary>
        /// <param name="payload">Text to encrypt</param>
        /// <param name="method">Payload encryption method</param>
        /// <param name="encodedPublicKey">Public key in encoded format</param>
        /// <param name="b58PublicKey">Public key in b58 format</param>
        /// <param name="password">Used to encrypt payload with aes as a payload_method. If none equals to empty password.</param>
        /// <remarks>Provide encodedPublicKey or b58PublicKey if use AbstractPayloadMethod.PubKey. If encodedPublicKey and b58PublicKey are used together then it should be the same public key.</remarks>
        /// <returns>HEXASTRING with encrypted payload</returns>
        public Task<Response<string>> PayloadEncryptAsync(string payload, AbstractPayloadMethod method, string encodedPublicKey = null, string b58PublicKey = null, string password = null)
        {
            var parameters = new
            {
                payload = payload?.ToHexString(),
                payload_method = method.ToString().ToLower(),
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey,
                pwd = password
            };
            return InvokeAsync<string>("payloadencrypt", Serialize(parameters));
        }

        /// <summary>decrypted text (a payload) using private keys in the wallet or a list of Passwords (used in "aes" encryption)</summary>
        /// <param name="payload">Encrypted data</param>
        /// <param name="passwords">Array of Strings (optional)</param>
        /// <remarks>If using one of private keys is able to decrypt payload then returns value "key" in payload_method and enc_pubkey contains encoded public key in HEXASTRING. If using one of passwords to decrypt payload then returns value "pwd" in payload_method and pwd contains password used</remarks>
        /// <returns>EncryptionResult object</returns>
        public Task<Response<EncryptionResult>> PayloadDecryptAsync(string payload, params string[] passwords)
        {
            var parameters = new
            {
                payload,
                pwds = passwords.Length > 0 ? passwords : null, //avoid sending empty JSON array because it is more beautiful
            };
            return InvokeAsync<EncryptionResult>("payloaddecrypt", Serialize(parameters));
        }

        /// <summary>Lists all active connections of this node</summary>
        /// <returns>Array with Connections objects</returns>
        public Task<Response<ConnectionInfo[]>> GetConnectionsAsync()
        {
            return InvokeAsync<ConnectionInfo[]>("getconnections");
        }

        /// <summary>Creates a new Private key and sotres it on the wallet, returning an enc_pubkey value</summary>
        /// <param name="encryptionType">Text to encrypt</param>
        /// <param name="name">Name to alias this new private key</param>
        /// <returns>PublicKey object</returns>
        public Task<Response<PublicKey>> AddNewKeyAsync(EncryptionType encryptionType, string name)
        {
            return InvokeAsync<PublicKey>("addnewkey", $"{{\"ec_nid\":{(int)encryptionType},\"name\":\"{name}\"}}");
        }

        /// <summary>Locks the Wallet if it has a password, otherwise wallet cannot be locked</summary>
        /// <returns>Returns a Boolean indicating if Wallet is locked. If false that means that Wallet has an empty password and cannot be locked</returns>
        public Task<Response<bool?>> LockAsync()
        {
            return InvokeAsync<bool?>("lock");
        }

        /// <summary>Unlocks a locked Wallet using "pwd" param</summary>
        /// <param name="pwd">Password</param>
        /// <returns>Returns a Boolean indicating if Wallet is unlocked after using pwd password</returns>
        public Task<Response<bool?>> UnlockAsync(string pwd)
        {
            return InvokeAsync<bool?>("unlock", $"{{\"pwd\":\"{pwd}\"}}");
        }

        /// <summary>Finds an operation by "ophash"</summary>
        /// <param name="pwd">New password</param>
        /// <returns>Returns a Boolean if Wallet password changed with new pwd password</returns>
        public Task<Response<bool?>> SetWalletPasswordAsync(string pwd)
        {
            return InvokeAsync<bool?>("setwalletpassword", $"{{\"pwd\":\"{pwd}\"}}");
        }

        /// <summary>Stops the node and the server. Closes all connections</summary>
        public Task<Response<bool?>> StopNodeAsync()
        {
            return InvokeAsync<bool?>("stopnode");
        }

        /// <summary>Starts the node and the server. Starts connection process</summary>
        public Task<Response<bool?>> StartNodeAsync()
        {
            return InvokeAsync<bool?>("startnode");
        }

        /// <summary>Signs a digest message using a public key</summary>
        /// <param name="digest">The digest message to sign</param>
        /// <param name="encodedPublicKey">Public key in Encoded format</param>
        /// <param name="b58PublicKey">Public key in Base 58 format (the same that Application Wallet exports)</param>
        /// <returns>Returns a MessageSigningInfo object</returns>
        public Task<Response<MessageSigningInfo>> SignMessageAsync(string digest, string encodedPublicKey = null, string b58PublicKey = null)
        {
            var parameters = new
            {
                digest = digest?.ToHexString(),
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey
            };
            return InvokeAsync<MessageSigningInfo>("signmessage", Serialize(parameters));
        }

        /// <summary>Verify if a digest message is signed by a public key</summary>
        /// <param name="digest">The digest message to sign</param>
        /// <param name="signature">HEXASTRING returned by SignMessage call</param>
        /// <param name="encodedPublicKey">Public key in Encoded format</param>
        /// <param name="b58PublicKey">Public key in Base 58 format (the same that Application Wallet exports)</param>
        /// <returns>True if verification is successfull, false - if verification fails.</returns>
        public async Task<Response<bool?>> VerifySignedMessageAsync(string digest, string signature, string encodedPublicKey = null, string b58PublicKey = null)
        {
            var encodedDigest = digest?.ToHexString();
            var parameters = new
            {
                digest = encodedDigest,
                enc_pubkey = encodedPublicKey,
                b58_pubkey = b58PublicKey,
                signature
            };
            var response = await InvokeAsync<MessageSigningInfo>("verifysign", Serialize(parameters));

            //Probably it is possible to get rid of checking if public keys match and skip additional wallet RPC call 'decodepubkey', but verification is important and there should be guarantee, that everything is done to get right results.
            //If wallet does not return clearly understandable sign (like boolean value) that verification was successful, then better to check if public keys match.
            var encodedKey = encodedPublicKey;
            if (encodedPublicKey == null && b58PublicKey != null)
            {
                var decodedPublicKeyResponse = await DecodePublicKeyAsync(b58PublicKey: b58PublicKey);
                if (decodedPublicKeyResponse.Error != null)
                {
                    return new Response<bool?>() { Id = decodedPublicKeyResponse.Id, Error = decodedPublicKeyResponse.Error, JsonRpc = decodedPublicKeyResponse.JsonRpc, Result = null };
                }
                encodedKey = decodedPublicKeyResponse.Result.EncodedPublicKey;
            }

            bool? result;
            if (response.Result != null)
            {
                result = response.Result.Signature == signature && response.Result.DigestHexString == encodedDigest && response.Result.EncodedPublicKey == encodedKey;
            }
            else
            {
                result = null; //According to JSON RPC specification, only one field should be null - Error or Result.
            }
            return new Response<bool?>() { Id = response.Id, Error = response.Error, JsonRpc = response.JsonRpc, Result = result };
        }

        /// <summary>Adds operations to a multioperation(or creates a new multioperation and adds new operations)</summary>
        /// <param name="senders">ARRAY of objects that will be Senders of the multioperation</param>
        /// <param name="receivers">ARRAY of objects that will be Receivers of the multioperation</param>
        /// <param name="changers">ARRAY of objects that will be accounts executing a changing info</param>
        /// <param name="autoNOperation">Will fill n_operation(if not provided). Only valid if wallet is ONLINE(no cold wallets)</param>
        /// <param name="rawOperations">HEXASTRING(optional) with previous multioperation.If is valid and contains a single multiopertion will add operations to this one, otherwise will generate a NEW MULTIOPERATION</param>
        /// <returns>Returns a MultiOperation object</returns>
        public Task<Response<MultiOperation>> MultioperationAddOperationAsync(IEnumerable<SenderBase> senders = null, IEnumerable<Receiver> receivers = null, IEnumerable<Changer> changers = null,
            bool? autoNOperation = null, string rawOperations = null)
        {
            var parameters = new
            {
                senders,
                receivers,
                changers,
                auto_n_operation = autoNOperation,
                rawoperations = rawOperations
            };
            return InvokeAsync<MultiOperation>("multioperationaddoperation", Serialize(parameters));
        }

        /// <summary>This method will sign a Multioperation found in a "rawoperations", must provide all n_operation info of each signer because can work in cold wallets</summary>
        /// <param name="rawoperations">HEXASTRING with 1 multioperation in Raw format</param>
        /// <param name="accountsAndKeys">Collection of objects with info about accounts and public keys to sign</param>
        /// <returns>Returns a MultiOperation object</returns>
        public Task<Response<MultiOperation>> MultiOperationSignOfflineAsync(string rawOperations, IEnumerable<AccountKeyPair> accountsAndKeys)
        {
            var parameters = new
            {
                rawoperations = rawOperations,
                accounts_and_keys = accountsAndKeys
            };
            return InvokeAsync<MultiOperation>("multioperationsignoffline", Serialize(parameters));
        }

        /// <summary>This method will sign a Multioperation found in a "rawoperations" based on current safebox state public keys</summary>
        /// <param name="rawOperations">HEXASTRING with 1 multioperation in Raw format</param>
        /// <returns>Returns a MultiOperation object</returns>
        public Task<Response<MultiOperation>> MultioperationSignOnlineAsync(string rawOperations)
        {
            return InvokeAsync<MultiOperation>("multioperationsignonline", $"{{\"rawoperations\":\"{rawOperations}\"}}");
        }

        /// <summary>This method will delete an operation included in a Raw operations object</summary>
        /// <param name="rawoperations">HEXASTRING with Raw Operations Object</param>
        /// <param name="index">Integer index to be deleted(from 0..count-1 )</param>
        /// <returns>Returns a RawOperation object</returns>
        public Task<Response<RawOperation>> OperationsDeleteAsync(string rawoperations, uint index)
        {
            return InvokeAsync<RawOperation>("operationsdelete", $"{{\"rawoperations\":\"{rawoperations}\",\"index\":{index}}}");
        }

        /// <summary>Created EPasa object from valid ePasa string</summary>
        /// <param name="ePasa">EPasa string</param>
        /// <returns>Returns a EPasa object</returns>
        public Task<Response<EPasa>> CheckEPasaAsync(string ePasa)
        {
            return InvokeAsync<EPasa>("checkepasa", $"{{\"account_epasa\":\"{ePasa}\"}}");
        }

        /// <summary>Created EPasa object from valid ePasa string</summary>
        /// <param name="account">Valid number or account name(Use @ for a PayToKey)</param>
        /// <param name="payloadMethod"></param>
        /// <param name="payload">HEXASTRING with the payload data</param>
        /// <param name="password">Will be used if PayloadMethod = Aes</param>
        /// <param name="encodingType"></param>
        /// <returns>Returns a EPasa object</returns>
        public Task<Response<EPasa>> ValidateEPasaAsync(string account, string payload, PayloadEncode? encodingType = PayloadEncode.String, PayloadMethod payloadMethod = PayloadMethod.None, string password = null)
        {
            var parameters = new
            {
                account,
                payload_method = payloadMethod.ToString().ToLower(),
                pwd = password,
                payload_encode = encodingType.ToString().ToLower(),
                payload = payload?.ToHexString()
            };
            return InvokeAsync<EPasa>("validateepasa", Serialize(parameters));
        }

        private static string Serialize<T>(T parametersObject)
        {
            var options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(parametersObject, options);
        }

        private async Task<Response<T>> InvokeAsync<T>(string methodName, string parameters = null)
        {
            var callId = Interlocked.Increment(ref _callId);

            var parametersElement = !string.IsNullOrEmpty(parameters) && parameters != "{}" ? $",\"params\":{parameters}" : string.Empty;
            const string jsonrpc = "2.0";

            var rpc = $"{{\"jsonrpc\":\"{jsonrpc}\",\"method\":\"{methodName}\"{parametersElement},\"id\":{callId}}}";
            var content = new StringContent(rpc, Encoding.UTF8)
            {
                Headers = { ContentType = new MediaTypeHeaderValue(mediaType: "application/json") }
            };

            Response<T> result;
            try
            {
                var response = await _httpClient.PostAsync(_url, content);
                //var forDebuggingPurposes = await response.Content.ReadAsStringAsync();
                var responseContentStream = await response.Content.ReadAsStreamAsync();
                var deserializeOptions = new JsonSerializerOptions()
                {
                    Converters =
                    {
                        new UnixTimeConverter(),
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    }
                };
                result = await JsonSerializer.DeserializeAsync<Response<T>>(responseContentStream, deserializeOptions);
            }
            catch (Exception exc)
            {
                result = new Response<T>() { Id = callId, Error = new Error(exc.Message, ErrorCode.InternalError), JsonRpc = jsonrpc };
            }
            return result;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
