// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Pascal.Wallet.Connector
{
    public static class PascalConnectorHelper
    {
        public static string AccountsToString(IEnumerable<uint> accounts)
        {
            var builder = new StringBuilder();
            var empty = true;
            foreach (var accountNumber in accounts)
            {
                if (empty)
                {
                    empty = false;
                }
                else
                {
                    builder.Append(',');
                }
                builder.Append(accountNumber);
            }
            return builder.ToString();
        }

        public static string ToHexString(this string originalString)
        {
            return !string.IsNullOrEmpty(originalString) ? Convert.ToHexString(Encoding.UTF8.GetBytes(originalString)) : originalString;
        }

        public static string FromHexString(this string hexaString)
        {
            return Encoding.UTF8.GetString(Convert.FromHexString(hexaString));
        }
    }
}
