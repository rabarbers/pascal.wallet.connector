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
            var sendingPascResponse = await connector.FindBlocksAsync(start: 532000, max: 1);
            if (sendingPascResponse.Result != null)
            {
                foreach(var account in sendingPascResponse.Result)
                {
                    Console.WriteLine(account.BlockNumber);
                }
            }
            else
            {
                Console.WriteLine($"Transaction failed. Error details: {sendingPascResponse.Error.Message}");
            }


            
        }
    }
}
