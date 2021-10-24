using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class JsonSchemaForNpmPackageJsonFilesExportsConverter : JsonConverter
    {
        public static readonly JsonSchemaForNpmPackageJsonFilesExportsConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(JsonSchemaForNpmPackageJsonFilesExports)) ||
                   (t == typeof(JsonSchemaForNpmPackageJsonFilesExports?));
        }

        public override object ReadJson(
            JsonReader reader,
            Type t,
            object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return new JsonSchemaForNpmPackageJsonFilesExports();
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new JsonSchemaForNpmPackageJsonFilesExports {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<FluffyPackageExportsEntryObject>(reader);
                    return new JsonSchemaForNpmPackageJsonFilesExports
                    {
                        FluffyPackageExportsEntryObject = objectValue
                    };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<PackageExportsEntry[]>(reader);
                    return new JsonSchemaForNpmPackageJsonFilesExports {AnythingArray = arrayValue};
            }

            throw new Exception("Cannot unmarshal type JsonSchemaForNpmPackageJsonFilesExports");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (JsonSchemaForNpmPackageJsonFilesExports) untypedValue;
            if (value.IsNull)
            {
                serializer.Serialize(writer, null);
                return;
            }

            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }

            if (value.AnythingArray != null)
            {
                serializer.Serialize(writer, value.AnythingArray);
                return;
            }

            if (value.FluffyPackageExportsEntryObject != null)
            {
                serializer.Serialize(writer, value.FluffyPackageExportsEntryObject);
                return;
            }

            throw new Exception("Cannot marshal type JsonSchemaForNpmPackageJsonFilesExports");
        }
    }
}
