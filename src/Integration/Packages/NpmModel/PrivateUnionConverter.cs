using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class PrivateUnionConverter : JsonConverter
    {
        public static readonly PrivateUnionConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(PrivateUnion)) || (t == typeof(PrivateUnion?));
        }

        public override object ReadJson(
            JsonReader reader,
            Type t,
            object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Boolean:
                    var boolValue = serializer.Deserialize<bool>(reader);
                    return new PrivateUnion {Bool = boolValue};
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    switch (stringValue)
                    {
                        case "false":
                            return new PrivateUnion {Enum = PrivateEnum.False};
                        case "true":
                            return new PrivateUnion {Enum = PrivateEnum.True};
                    }

                    break;
            }

            throw new Exception("Cannot unmarshal type PrivateUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (PrivateUnion) untypedValue;
            if (value.Bool != null)
            {
                serializer.Serialize(writer, value.Bool.Value);
                return;
            }

            if (value.Enum != null)
            {
                switch (value.Enum)
                {
                    case PrivateEnum.False:
                        serializer.Serialize(writer, "false");
                        return;
                    case PrivateEnum.True:
                        serializer.Serialize(writer, "true");
                        return;
                }
            }

            throw new Exception("Cannot marshal type PrivateUnion");
        }
    }
}
