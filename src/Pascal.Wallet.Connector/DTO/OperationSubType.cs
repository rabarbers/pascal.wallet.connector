// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin

namespace Pascal.Wallet.Connector.DTO
{
	public enum OperationSubType
    {
		TransactionSender = 11,
		TransactionReceiver = 12,
		BuyTransactionBuyer = 13,
		BuyTransactionTarget = 14,
		BuyTransactionSeller = 15,
		ChangeKey = 21,
		Recover = 31,
		ListAccountForPublicSale = 41,
		ListAccountForPrivateSale = 42,
		DelistAccount = 51,
		BuyAccountBuyer = 61,
		BuyAccountTarget = 62,
		BuyAccountSeller = 63,
		ChangeKeySigned = 71,
		ChangeAccountInfo = 81,
		DataOperationSender = 102,
	}
}
