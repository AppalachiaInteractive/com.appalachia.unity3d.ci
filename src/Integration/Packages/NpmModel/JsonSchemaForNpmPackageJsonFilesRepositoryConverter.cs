using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class JsonSchemaForNpmPackageJsonFilesRepositoryConverter : JsonConverter
    {
        public static readonly JsonSchemaForNpmPackageJsonFilesRepositoryConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(JsonSchemaForNpmPackageJsonFilesRepository)) ||
                   (t == typeof(JsonSchemaForNpmPackageJsonFilesRepository?));
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
                    return new JsonSchemaForNpmPackageJsonFilesRepository {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PurpleRepository>(reader);
                    return new JsonSchemaForNpmPackageJsonFilesRepository {PurpleRepository = objectValue};
            }

            throw new Exception("Cannot unmarshal type JsonSchemaForNpmPackageJsonFilesRepository");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (JsonSchemaForNpmPackageJsonFilesRepository) untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }

            if (value.PurpleRepository != null)
            {
                serializer.Serialize(writer, value.PurpleRepository);
                return;
            }

            throw new Exception("Cannot marshal type JsonSchemaForNpmPackageJsonFilesRepository");
        }
    }
}
