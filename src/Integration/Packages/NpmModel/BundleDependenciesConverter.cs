using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class BundleDependenciesConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly BundleDependenciesConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(BundleDependencies)) || (t == typeof(BundleDependencies?));
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
                    return new BundleDependencies {Bool = boolValue};
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<string[]>(reader);
                    return new BundleDependencies {StringArray = arrayValue};
            }

            throw new Exception("Cannot unmarshal type BundleDependencies");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (BundleDependencies) untypedValue;
            if (value.Bool != null)
            {
                serializer.Serialize(writer, value.Bool.Value);
                return;
            }

            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray);
                return;
            }

            throw new Exception("Cannot marshal type BundleDependencies");
        }
    }
}
