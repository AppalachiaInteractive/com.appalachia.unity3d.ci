using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class TypeEnumConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly TypeEnumConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(TypeEnum)) || (t == typeof(TypeEnum?));
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
                case "commonjs":
                    return TypeEnum.Commonjs;
                case "module":
                    return TypeEnum.Module;
                case "library":
                    return TypeEnum.Library;
            }

            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (TypeEnum) untypedValue;
            switch (value)
            {
                case TypeEnum.Commonjs:
                    serializer.Serialize(writer, "commonjs");
                    return;
                case TypeEnum.Module:
                    serializer.Serialize(writer, "module");
                    return;
                case TypeEnum.Library:
                    serializer.Serialize(writer, "library");
                    return;
            }

            throw new Exception("Cannot marshal type TypeEnum");
        }
    }
}
