using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class JsonSchemaForNpmPackageJsonFilesBugsConverter : JsonConverter
    {
        public static readonly JsonSchemaForNpmPackageJsonFilesBugsConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(JsonSchemaForNpmPackageJsonFilesBugs)) ||
                   (t == typeof(JsonSchemaForNpmPackageJsonFilesBugs?));
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
                    return new JsonSchemaForNpmPackageJsonFilesBugs {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<FluffyBugs>(reader);
                    return new JsonSchemaForNpmPackageJsonFilesBugs {FluffyBugs = objectValue};
            }

            throw new Exception("Cannot unmarshal type JsonSchemaForNpmPackageJsonFilesBugs");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (JsonSchemaForNpmPackageJsonFilesBugs) untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }

            if (value.FluffyBugs != null)
            {
                serializer.Serialize(writer, value.FluffyBugs);
                return;
            }

            throw new Exception("Cannot marshal type JsonSchemaForNpmPackageJsonFilesBugs");
        }
    }
}
