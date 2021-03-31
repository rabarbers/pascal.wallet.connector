// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary></summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/rpc#operation-object">https://www.pascalcoin.org/development/rpc#operation-object</see></remarks>
    public enum OperationType
    {
        BlockchainReward,
        Transaction,
        ChangeKey,
        RecoverFounds,
        ListAccountForSale,
        DelistAccount,
        BuyAccount,
        ChangeKeySignedByAnotherAccount,
        ChangeAccountInfo,
        Multioperation,
        DataOperation
    }
}
