using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class PackageExportsEntryConverter : JsonConverter
    {
        public static readonly PackageExportsEntryConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(PackageExportsEntry)) || (t == typeof(PackageExportsEntry?));
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
                    return new PackageExportsEntry();
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new PackageExportsEntry {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue =
                        serializer.Deserialize<PackageExportsEntryPackageExportsEntryObject>(reader);
                    return new PackageExportsEntry
                    {
                        PackageExportsEntryPackageExportsEntryObject = objectValue
                    };
            }

            throw new Exception("Cannot unmarshal type PackageExportsEntry");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (PackageExportsEntry) untypedValue;
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

            if (value.PackageExportsEntryPackageExportsEntryObject != null)
            {
                serializer.Serialize(writer, value.PackageExportsEntryPackageExportsEntryObject);
                return;
            }

            throw new Exception("Cannot marshal type PackageExportsEntry");
        }
    }
}
