using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class JsonSchemaForNpmPackageJsonFilesWorkspacesConverter : JsonConverter
    {
        public static readonly JsonSchemaForNpmPackageJsonFilesWorkspacesConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(JsonSchemaForNpmPackageJsonFilesWorkspaces)) ||
                   (t == typeof(JsonSchemaForNpmPackageJsonFilesWorkspaces?));
        }

        public override object ReadJson(
            JsonReader reader,
            Type t,
            object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PurpleWorkspaces>(reader);
                    return new JsonSchemaForNpmPackageJsonFilesWorkspaces {PurpleWorkspaces = objectValue};
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<string[]>(reader);
                    return new JsonSchemaForNpmPackageJsonFilesWorkspaces {StringArray = arrayValue};
            }

            throw new Exception("Cannot unmarshal type JsonSchemaForNpmPackageJsonFilesWorkspaces");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (JsonSchemaForNpmPackageJsonFilesWorkspaces) untypedValue;
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray);
                return;
            }

            if (value.PurpleWorkspaces != null)
            {
                serializer.Serialize(writer, value.PurpleWorkspaces);
                return;
            }

            throw new Exception("Cannot marshal type JsonSchemaForNpmPackageJsonFilesWorkspaces");
        }
    }
}
