using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class NpmPackageExportsConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly NpmPackageExportsConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(NpmPackageExports)) || (t == typeof(NpmPackageExports?));
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
                    return new NpmPackageExports();
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new NpmPackageExports {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PurplePackageExportsEntryObject>(reader);
                    return new NpmPackageExports {PurplePackageExportsEntryObject = objectValue};
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<PackageExportsEntry[]>(reader);
                    return new NpmPackageExports {AnythingArray = arrayValue};
            }

            throw new Exception("Cannot unmarshal type NpmPackageExports");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (NpmPackageExports) untypedValue;
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

            if (value.PurplePackageExportsEntryObject != null)
            {
                serializer.Serialize(writer, value.PurplePackageExportsEntryObject);
                return;
            }

            throw new Exception("Cannot marshal type NpmPackageExports");
        }
    }
}
