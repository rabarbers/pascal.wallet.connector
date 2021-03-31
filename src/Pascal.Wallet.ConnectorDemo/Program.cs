using Pascal.Wallet.Connector.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Pascal.Wallet.Connector.Demo
{
    class Program
    {
        static async Task Main()
        {
            
            using var client = new PascalConnector(port: 4203);

            var sendingPascResponse = await client.SendToAsync(senderAccount: 796500, receiverAccount: 834853, amount: 5M, fee: 0.0001M, payload: "Thanks for good work!", payloadMethod: PayloadMethod.None);
            if (sendingPascResponse.Result != null)
            {
                //access information about the operation at sendingPascResponse.Result
                Console.WriteLine($"Successfully sent 5 Pascals to a friend. Operation details: {sendingPascResponse.Result.Description}");
            }
            else
            {
                Console.WriteLine($"Transaction failed, error details: {sendingPascResponse.Error?.Message}");
            }


            var identifier = new Guid("AAA19787-F847-4323-8987-2E652F593BCE");
            var sendingDataResponse = await client.SendDataAsync(senderAccount: 796500, receiverAccount: 834853, guid: identifier.ToString(),
                dataType: DataType.ChatMessage, payload: "This is a free chat message on Pascal blockchain! Find out more at https://www.pascalcoin.org/");
            if (sendingDataResponse.Result != null)
            {
                Console.WriteLine($"Successfully sent important information to a friend. Operation details: {sendingDataResponse.Result.Description}");
            }
            else
            {
                Console.WriteLine($"Failed to chat with a friend, error details: {sendingDataResponse.Error?.Message}");
            }


            //Signing requires that in your wallet exists private key for this public key. Wallet selects right private key, you do not expose private keys in the code.
            //Assuming, that message and public key is known to both parties
            var message = "This is digest message for signing!";
            var userAccountPublicKey = "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8";
            var signingResponse = await client.SignMessageAsync(digest: message, b58PublicKey: userAccountPublicKey);
            var userSignature = signingResponse.Result?.Signature;

            //when other user receives your signature on his computer, he can verify if the message was signed using your private key
            var verificationResponse = await client.VerifySignedMessageAsync(digest: message, signature: userSignature, b58PublicKey: userAccountPublicKey);
            if (verificationResponse.Result)
            {
                Console.WriteLine("Verification successfull");
            }
            else
            {
                Console.WriteLine($"Verification failed, error details: {verificationResponse.Error?.Message}");
            }


            //in this example person has several accounts in his wallet and he combines 2 Pascals from 2 different accounts and sends to the receiver account
            //in MultiOperation can participate several receivers, but in this example we use only 1 receiver
            var senders = new List<Sender>
            {
                //If senders send more than receivers expect, then the difference goes to transaction fees. In this example first sender will pay transaction fee: 0.0001 Pascals.
                new Sender(accountNumber: 796500, amount: 1.0001M),
                new Sender(accountNumber: 1141769, amount: 1)
            };
            var receivers = new List<Receiver> { new Receiver(accountNumber: 834853, amount: 2, payload: "Enjoy 2 Pascals!") };
            var multiOpResponse = await client.MultioperationAddOperationAsync(senders: senders, receivers: receivers);

            //MultiOperation needs to be signed before it can be executed. Signing is done automatically by the wallet.
            var signedMultiOpResponse = await client.MultioperationSignOnlineAsync(multiOpResponse.Result?.RawOperations);

            ///Signed MultiOperation can be executed
            var executedMultiOpResponse = await client.ExecuteOperationsAsync(signedMultiOpResponse.Result?.RawOperations);
            if (executedMultiOpResponse.Result?.SingleOrDefault()?.Valid ?? false)
            {
                Console.WriteLine("Sent 2 Pascals from 2 different accounts to a friend.");
            }
            else
            {
                Console.WriteLine($"MultiOperation failed, error details: {executedMultiOpResponse.Result?.SingleOrDefault()?.Errors}");
            }


            //creating Pascal sending transaction offline (cold wallet) and then executing it (lastNOperations should be set to N.Op. of the account that is shown in the wallet).
            var coldWalletSendingResponse = await client.SignSendToAsync(senderAccount: 796500, receiverAccount: 834853, amount: 1, lastNOperation: 34,
               senderB58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8",
               targetB58PublicKey: "JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv");

            var coldWalletSendingExecutionResponse = await client.ExecuteOperationsAsync(coldWalletSendingResponse.Result.RawOperations);
            if (coldWalletSendingExecutionResponse.Result?.SingleOrDefault()?.Valid ?? false)
            {
                Console.WriteLine("Successfully executed Pascals sending transaction crafted in cold wallet");
            }
            else
            {
                Console.WriteLine($"Cold wallet operation failed, error details: {coldWalletSendingExecutionResponse.Result?.SingleOrDefault()?.Errors}");
            }


        }
    }
}
