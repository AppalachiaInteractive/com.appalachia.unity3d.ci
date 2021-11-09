using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class MinMaxLengthCheckConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly MinMaxLengthCheckConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return t == typeof(string);
        }

        public override object ReadJson(
            JsonReader reader,
            Type t,
            object existingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            if ((value.Length >= 1) && (value.Length <= 214))
            {
                return value;
            }

            throw new Exception("Cannot unmarshal type string");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (string) untypedValue;
            if ((value.Length >= 1) && (value.Length <= 214))
            {
                serializer.Serialize(writer, value);
                return;
            }

            throw new Exception("Cannot marshal type string");
        }
    }
}
