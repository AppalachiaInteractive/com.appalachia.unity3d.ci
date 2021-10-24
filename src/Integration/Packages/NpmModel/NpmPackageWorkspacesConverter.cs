using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class NpmPackageWorkspacesConverter : JsonConverter
    {
        public static readonly NpmPackageWorkspacesConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(NpmPackageWorkspaces)) || (t == typeof(NpmPackageWorkspaces?));
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
                    var objectValue = serializer.Deserialize<FluffyWorkspaces>(reader);
                    return new NpmPackageWorkspaces {FluffyWorkspaces = objectValue};
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<string[]>(reader);
                    return new NpmPackageWorkspaces {StringArray = arrayValue};
            }

            throw new Exception("Cannot unmarshal type NpmPackageWorkspaces");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (NpmPackageWorkspaces) untypedValue;
            if (value.StringArray != null)
            {
                serializer.Serialize(writer, value.StringArray);
                return;
            }

            if (value.FluffyWorkspaces != null)
            {
                serializer.Serialize(writer, value.FluffyWorkspaces);
                return;
            }

            throw new Exception("Cannot marshal type NpmPackageWorkspaces");
        }
    }
}
