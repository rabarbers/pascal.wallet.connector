// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.
// Documentation thanks to PIP-0016 https://www.pascalcoin.org/development/pips/pip-0016

namespace Pascal.Wallet.Connector.DTO
{
    /// <summary>Used by Layer-2 protocol to distinguish different types of messages</summary>
    /// <remarks><see href="https://www.pascalcoin.org/development/pips/pip-0016">https://www.pascalcoin.org/development/pips/pip-0016</see></remarks>
    public enum DataType
    {
        ChatMessage,
        PrivateMessage,
        File
    }
}
