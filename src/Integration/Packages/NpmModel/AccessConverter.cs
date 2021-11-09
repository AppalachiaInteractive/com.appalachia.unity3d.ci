using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class AccessConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly AccessConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(Access)) || (t == typeof(Access?));
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
                case "public":
                    return Access.Public;
                case "restricted":
                    return Access.Restricted;
            }

            throw new Exception("Cannot unmarshal type Access");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (Access) untypedValue;
            switch (value)
            {
                case Access.Public:
                    serializer.Serialize(writer, "public");
                    return;
                case Access.Restricted:
                    serializer.Serialize(writer, "restricted");
                    return;
            }

            throw new Exception("Cannot marshal type Access");
        }
    }
}
