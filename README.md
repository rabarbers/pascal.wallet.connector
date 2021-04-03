# Pascal.Wallet.Connector
Based on the source code of [NPascalCoin](https://github.com/Sphere10/NPascalCoin) and Pascal Wallet JSON RPC API [Specification & Guidelines](https://www.pascalcoin.org/development/rpc).  
Pascal.Wallet.Connector is a .NET 5 library to call PascalCoin Wallet JSON RPC API method calls. Pascal.Wallet.Connector is implemented in C# and does not depend on other packages or libraries. The main idea of Pascal.Wallet.Connector is to improve user engagement in Pascal app development and to improve the documentation of the Pascal Wallet JSON RPC API methods. Code fragments from this repository can easily be copied. This repository has good Unit Tests which acts like documentation demonstrating how to use Pascal.Wallet.Connector library.
## .NET5 library to call Pascal full node Wallet JSON RPC API methods
Link to Nuget.org package...
### Create and configure PascalConnector
Pascal.Wallet.Connector library contains PascalConnector class which allows you to connect to the Pascal full node Wallet. The PascalConnector is not an independent Pascal network client. Pascal full node Wallet is the client for the Pascal network and PascalConnector connects to the Wallet to call JSON RPC API methods supported by the Wallet. It means that the Wallet should be kept open to be able to use PascalConnector.
```c#
using var connector = new PascalConnector(address: "127.0.0.1", port: 4003);
```

### Send Pascals from one account to another account
In Pascal network, every account can send 1 free transaction per block. Other transactions in the same block costs minimum 0.0001 Pascals or 1 Molina. Pascals can be send only from the accounts you own (you need to have private keys in the Wallet).
```c#
var sendingPascResponse = await connector.SendToAsync(senderAccount: 1141769, receiverAccount: 834853,
    amount: 1.2345M, fee: 0, payload: "Thanks for good work!", payloadMethod: PayloadMethod.None);
if (sendingPascResponse.Result != null)
{
    var amount = sendingPascResponse.Result.Receivers[0].Amount;
    var description = sendingPascResponse.Result.Description;
    Console.WriteLine($"Successfully sent {amount} Pascals to a friend. Operation details: {description}");
}
else
{
    Console.WriteLine($"Transaction failed. Error details: {sendingPascResponse.Error.Message}");
}
```

### Send data from one account to another account
Pascal network supports feature called 'Data Operations' which allows you to exchange data between accounts. Data is transmitted using payload field of the operation. Payload field can also be used in Pascal sending transactions (RPC API method SendToAsync). SendToAsync and SendDataAsync can be used for free (no operation fee for 1 operation per account per block). SendToAsync has minimum Pascals sending restriction set to 0.0001 Pasc or 1 Molina, but SendDataAsync does not have such restriction, therefore SendDataAsync can be used completely for free. SendDataAsync contains additional functionality useful to transmit data, more details in [PIP-0033](https://www.pascalcoin.org/development/pips/pip-0033).
```c#
var identifier = new Guid("AAA19787-F847-4323-8987-2E652F593BCE");
var data = "This is a free chat message on Pascal blockchain! Find out more at https://www.pascalcoin.org";
var sendingDataResponse = await connector.SendDataAsync(senderAccount: 796500, receiverAccount: 834853, fee: 0,
    guid: identifier.ToString(), dataType: DataType.ChatMessage, payload: data);
if (sendingDataResponse.Result != null)
{
    var description = sendingDataResponse.Result.Description;
    Console.WriteLine($"Successfully sent important information to a friend. Operation details: {description}");
}
else
{
    Console.WriteLine($"Failed to chat with a friend, error details: {sendingDataResponse.Error.Message}");
}
```

### Sign messages and verify signatures
Signature verification proves ownership of private key for the specified public key. Pascal accounts are not involved in message signing and signature verification process. But Pascal accounts are controlled using private-public key pairs, therefore message signing and verification can be used to determine if the user controls (owns) the account.
Signing requires that in the wallet exists private key for the choosen public key. The Wallet picks the respective private key for the choosen public key. **DO NOT expose private keys in the C# code.**
Digest message and public key should be known to both parties.
```c#
//user1
var message = "This is digest message for signing!";
var user1AccountPublicKey = "3GhhbosT1b4mpDh1KkNzstujECTn2JS31cArR5wwXif97c71kFmmHNmida3W7Ax8MuKATYSQeJmpDVvseELM1Q7bxbTPTYK5fspG8p";
var signingResponse = await connector.SignMessageAsync(digest: message, b58PublicKey: user1AccountPublicKey);
var userSignature1 = signingResponse.Result?.Signature;
```
When the user2 receives the signature from the user1, the user2 can verify if the message was signed using the private key which corresponds to the public key used in this example.
```c#
//user2
var verificationResponse = await connector.VerifySignedMessageAsync(digest: message, signature: userSignature1,
    b58PublicKey: user1AccountPublicKey);
if (verificationResponse.Result ?? false)
{
    Console.WriteLine("Verification successfull");
}
else
{
    Console.WriteLine($"Verification failed, error details: {verificationResponse.Error.Message}");
}
```

### Sending Pascals from several accounts to several accounts
In this example, the user has several accounts in his wallet and he sends 2 Pascals combined from 2 different accounts to the receiver account.
Several receivers can participate in MultiOperation, but in this example only 1 receiver is used.
```c#
var senders = new List<Sender>
{
    //If senders send more than receivers expect, then the difference goes to transaction fees
    //In this example first sender will pay transaction fee: 0.0001 Pascals
    new Sender(accountNumber: 1141769, amount: 1.0001M),
    new Sender(accountNumber: 796500, amount: 1)
};
var receivers = new List<Receiver> { new Receiver(accountNumber: 834853, amount: 2, payload: "Enjoy 2 Pascals!") };
var multiOpResponse = await connector.MultioperationAddOperationAsync(senders: senders, receivers: receivers);

//MultiOperation needs to be signed before it can be executed
//Wallet knows how to pick the right keys and sign the MultiOperation
var signedMultiOpResponse = await connector.MultioperationSignOnlineAsync(multiOpResponse.Result?.RawOperations);

///Signed MultiOperation can be executed
var executedMultiOpResponse = await connector.ExecuteOperationsAsync(signedMultiOpResponse.Result?.RawOperations);
if (executedMultiOpResponse.Result?.SingleOrDefault()?.Valid ?? false)
{
    Console.WriteLine("Sent 2 Pascals from 2 different accounts to a friend.");
}
else
{
    Console.WriteLine($"MultiOperation failed. Error details: {executedMultiOpResponse.Result?.SingleOrDefault()?.Errors}");
}
```

### Create Pascal sending transaction offline (cold wallet) and then execute it on Pascal network
The Wallet is called 'cold wallet' if it operates offline. Pascal Wallet supports creating Operations offline. Such operations can be brought to Pascal Network online Wallet ('hot wallet'). Pascal network prevents 'double spending' by maintaining NOperations field (in Wallet Classic UI version it is represented as 'N.Op.' and it is shown near each account. When the account executes the operation, the NOperation of the account is increased. To be able to create valid operations offline you need to be sure that other operations are not executed with the account (the account NOperations value does not change) until your offline operation is executed on Pascal network. In this example, the parameter 'lastNOperations' should be set to 'N.Op.' of the account that is shown in the Wallet UI. It is possible to create several offline operations and execute it later, but it involves precise planning on how the NOperations value of the account will change.
```c#
//executed offline (cold wallet)
var coldWalletSendingResponse = await connector.SignSendToAsync(
    senderAccount: 1141769, receiverAccount: 834853, amount: 1, lastNOperation: 20, fee: 0,
    senderB58PublicKey: "3GhhbonUn567anoPfv7UhFsTqwtuKom295sb34jxxWYC1e4DnKSmjc4wQvfEnaJcDK3F4pKeyiHUzwyzuvx5cgKLvPGm7pXsByQcfL",
    targetB58PublicKey: "3GhhbovneptEFJGFPvFprcVE2hDfea2mXLgHDFRuvsfB1kqS6LooC1s4iudS9LpTsgUD4h9Jrufpn1N77XCpaMiU8XhEWYBYv5hJXr"
);
var operations = coldWalletSendingResponse.Result?.RawOperations;

//executed online
var coldWalletSendingExecutionResponse = await connector.ExecuteOperationsAsync(operations);
var isValidOperation = coldWalletSendingExecutionResponse.Result?.SingleOrDefault()?.Valid ?? false;
if (isValidOperation)
{
    Console.WriteLine("Successfully executed Pascals sending transaction that was created in cold wallet");
}
else
{
    var errors = coldWalletSendingExecutionResponse.Result?.SingleOrDefault()?.Errors;
    Console.WriteLine($"Cold wallet operation failed. Error details: {errors}");
}
```

### Error checking
According to [JSON RPC Specification](https://www.jsonrpc.org/specification) if the RPC method call is successfull,
then the field 'Result' of the response object is not null and the 'Error' is null.
If the RPC method call is not successfull, then the field 'Error' is not null and 'Result' is null.
Note that some objects like RawOperations contains inner data fields with error messages and validity information.

### Pascal.Wallet.Connector supported features
* Pascal Wallet JSON RPC API methods [(Documentation for Wallet version 3.0)](https://www.pascalcoin.org/development/rpc).  
* Updated data structures (Wallet version 5.4.)  
* DataOperation methods: SendDataAsync, SignDataAsync, FindDataOperationsAsync. More details in [PIP-0033](https://www.pascalcoin.org/development/pips/pip-0033).  

### Known issues
* Cold wallet operation SignSendToAsync does not recognize accounts in MainNet.  
* Operation.Amount for SendToAsync returns sent Pasc, but for SendDataAsync it returns sent Pasc + transaction fee.  
* It is not clear if Block.Nonce field represents meaningful value.  
* GetWalletAccountsAsync returns empty array if parameter encodedPublicKey or b58PublicKey is provided.  
* Wallet RPC methods 'checkepasa' and 'validateepasa' returns response object which does not correspond JSON RPC specification and field 'account' sometimes can return numeric value, sometimes - string. Currently Pascal.Wallet.Connector does not support these methods.

### For technical support contact Rabarbers and be polite

## Roadmap
* RPC EPasa methods: 'checkepasa' and 'validateepasa' according to [Wallet v5.4 CHANGELOG](https://github.com/PascalCoin/PascalCoin/blob/master/CHANGELOG.md) and [PIP-0027](https://www.pascalcoin.org/development/pips/pip-0027).  
* Update RPC method signatures taking into consideration all the changes documented in [Wallet v5.4 CHANGELOG](https://github.com/PascalCoin/PascalCoin/blob/master/CHANGELOG.md).  
* RPC method 'paytokey' when it will be supported in the Wallet according to [PIP-0041](https://www.pascalcoin.org/development/pips/pip-0041).  

## Feedback & Donations
pascal.wallet.connector account 834853-50.  
Feedback can be send as DataOperations (using method SendDataAsync). Donations can be send as transactions (using method SendToAsync).  
If you need an account to test your operations, you are welcome to send your test messages to pascal.wallet.connector account.
