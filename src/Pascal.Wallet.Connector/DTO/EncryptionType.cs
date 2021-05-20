// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Type of key used for public/private key encryption.</summary>
    public enum EncryptionType: ushort
    {
        Secp256k1 = 714,
        Secp384r1 = 715,
        Secp283k1 = 729,
        secp521r1 = 716
    }
}
