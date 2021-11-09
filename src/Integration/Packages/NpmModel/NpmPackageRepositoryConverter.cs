using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class NpmPackageRepositoryConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly NpmPackageRepositoryConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(NpmPackageRepository)) || (t == typeof(NpmPackageRepository?));
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
                    return new NpmPackageRepository {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<FluffyRepository>(reader);
                    return new NpmPackageRepository {FluffyRepository = objectValue};
            }

            throw new Exception("Cannot unmarshal type NpmPackageRepository");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (NpmPackageRepository) untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }

            if (value.FluffyRepository != null)
            {
                serializer.Serialize(writer, value.FluffyRepository);
                return;
            }

            throw new Exception("Cannot marshal type NpmPackageRepository");
        }
    }
}
