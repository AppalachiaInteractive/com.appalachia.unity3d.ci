using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class PrivateEnumConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly PrivateEnumConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(PrivateEnum)) || (t == typeof(PrivateEnum?));
        }

        public override object ReadJson(
            JsonReader reader,
            Type t,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "false":
                    return PrivateEnum.False;
                case "true":
                    return PrivateEnum.True;
            }

            throw new Exception("Cannot unmarshal type PrivateEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (PrivateEnum) untypedValue;
            switch (value)
            {
                case PrivateEnum.False:
                    serializer.Serialize(writer, "false");
                    return;
                case PrivateEnum.True:
                    serializer.Serialize(writer, "true");
                    return;
            }

            throw new Exception("Cannot marshal type PrivateEnum");
        }
    }
}
