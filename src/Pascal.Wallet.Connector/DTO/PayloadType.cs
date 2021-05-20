// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Based on source code of NPascalCoin https://github.com/Sphere10/NPascalCoin
// Documentation thanks to PIP-0027 https://www.pascalcoin.org/development/pips/pip-0027

using System;

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>The PayloadType describes the encryption and encoding of the Payload</summary>
    [Flags]
    public enum PayloadType
    {
        /// <summary>Payload encryption and encoding method not specified</summary>
        NonDeterministic = 0,

        /// <summary>Unencrypted, public payload</summary>
        Public = 0x01,

        /// <summary>ECIES encrypted using recipient accounts public key</summary>
        RecipientKeyEncrypted = 0x02,

        /// <summary>ECIES encrypted using sender accounts public key</summary>
        SenderKeyEncrypted = 0x04,

        /// <summary>AES encrypted using password</summary>
        PasswordEncrypted = 0x08,

        /// <summary>Payload data encoded in ASCII</summary>
        AsciiFormatted = 0x10,

        /// <summary>Payload data encoded in HEX</summary>
        HexFormatted = 0x20,

        /// <summary>Payload data encoded in Base58</summary>
        Base58Formatted = 0x40,

        /// <summary>E-PASA addressed by account name (not number)</summary>
        AddressedByName = 0x80,
    }
}
