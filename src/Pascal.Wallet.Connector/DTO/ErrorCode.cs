// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to pascalcoin.org https://www.pascalcoin.org/development/rpc

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Data structure used in DataOperations</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#error-codes">https://www.pascalcoin.org/development/rpc#error-codes</see></remarks>
    public enum ErrorCode
    {
        InternalError = 100,
        MethodNotFound = 1001,
        InvalidAccount = 1002,
        InvalidBlock = 1003,
        InvalidOperation = 1004,
        InvalidPublicKey = 1005,
        NotFound = 1010,
        WalletIsPasswordProtected = 1015,
        InvalidData = 1016
    }
}
