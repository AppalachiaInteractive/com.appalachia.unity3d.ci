using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class BinConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly BinConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(Bin)) || (t == typeof(Bin?));
        }

        public override object ReadJson(
            JsonReader reader,
            Type t,
            object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Bin {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<Dictionary<string, string>>(reader);
                    return new Bin {StringMap = objectValue};
            }

            throw new Exception("Cannot unmarshal type Bin");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Bin) untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }

            if (value.StringMap != null)
            {
                serializer.Serialize(writer, value.StringMap);
                return;
            }

            throw new Exception("Cannot marshal type Bin");
        }
    }
}
