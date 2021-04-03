// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

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
            using var connector = new PascalConnector(address: "127.0.0.1", port: 4003);

            //Send Pascals from one account to another account
            var sendingPascResponse = await connector.SendToAsync(senderAccount: 1141769, receiverAccount: 834853, amount: 1.2345M, fee: 0, payload: "Thanks for good work!", payloadMethod: PayloadMethod.None);
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


            //Send data from one account to another account
            var identifier = new Guid("AAA19787-F847-4323-8987-2E652F593BCE");
            var data = "This is a free chat message on Pascal blockchain! Find out more at https://www.pascalcoin.org";
            var sendingDataResponse = await connector.SendDataAsync(senderAccount: 796500, receiverAccount: 834853, fee: 0, guid: identifier.ToString(), dataType: DataType.ChatMessage, payload: data);
            if (sendingDataResponse.Result != null)
            {
                var description = sendingDataResponse.Result.Description;
                Console.WriteLine($"Successfully sent important information to a friend. Operation details: {description}");
            }
            else
            {
                Console.WriteLine($"Failed to chat with a friend, error details: {sendingDataResponse.Error.Message}");
            }


            /* Sign messages and verify signatures
             * Signature verification proves ownership of private key for the specified public key. Pascal accounts are not involved in message signing and signature verification process.
             * But Pascal accounts are controlled using private-public key pairs, therefore message signing and verification can be used to determine if the user controls (owns) the account.
             * Signing requires that in the wallet exists private key for the choosen public key. The Wallet picks the respective private key for the choosen public key.
             * DO NOT expose private keys in the C# code. Digest message and public key should be known to both parties. */
            //user1
            var message = "This is digest message for signing!";
            var user1AccountPublicKey = "3GhhbosT1b4mpDh1KkNzstujECTn2JS31cArR5wwXif97c71kFmmHNmida3W7Ax8MuKATYSQeJmpDVvseELM1Q7bxbTPTYK5fspG8p";
            var signingResponse = await connector.SignMessageAsync(digest: message, b58PublicKey: user1AccountPublicKey);
            var userSignature1 = signingResponse.Result?.Signature;
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


            //Sending Pascals from several accounts to several accounts
            /* In this example, the user has several accounts in his wallet and he sends 2 Pascals combined from 2 different accounts to the receiver account.
             * Several receivers can participate in MultiOperation, but in this example only 1 receiver is used. */
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

            //Signed MultiOperation can be executed
            var executedMultiOpResponse = await connector.ExecuteOperationsAsync(signedMultiOpResponse.Result?.RawOperations);
            if (executedMultiOpResponse.Result?.SingleOrDefault()?.Valid ?? false)
            {
                Console.WriteLine("Sent 2 Pascals from 2 different accounts to a friend.");
            }
            else
            {
                Console.WriteLine($"MultiOperation failed. Error details: {executedMultiOpResponse.Result?.SingleOrDefault()?.Errors}");
            }


            /* Create Pascal sending transaction offline (cold wallet) and then execute it on Pascal network
             * Pascal network prevents 'double spending' by maintaining NOperations field (in Wallet Classic UI version it is represented as 'N.Op.' and it is shown near each account.
             * When the account executes the operation, the NOperation of the account is increased. To be able to create valid operations offline you need to be sure that other
             * operations are not executed with the account (the account NOperations value does not change) until your offline operation is executed on Pascal network.
             * In this example, the parameter 'lastNOperations' should be set to 'N.Op.' of the account that is shown in the Wallet UI. 
             * It is possible to create several offline operations and execute it later, but it involves precise planning on how the NOperations value of the account will change. */
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
        }
    }
}
