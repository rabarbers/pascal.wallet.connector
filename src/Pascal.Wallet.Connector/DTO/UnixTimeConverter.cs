// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pascal.Wallet.Connector.DTO
{
    public class UnixTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var unixTimeStamp = reader.GetUInt32();
            var time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return time.AddSeconds(unixTimeStamp);
        }

        public override void Write(Utf8JsonWriter writer, DateTime time, JsonSerializerOptions options)
        {
            var unixTimeStamp = time.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            writer.WriteNumberValue(unixTimeStamp);
        }
    }
}
