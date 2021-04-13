// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin

using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum AccountState
	{
		/// <summary>Normal account - not listed for sale</summary>
		Normal,
		/// <summary>Listed for sale</summary>
		Listed,
		/// <summary></summary>
		Coin_Swap,
		/// <summary></summary>
		Account_Swap
	}
}
