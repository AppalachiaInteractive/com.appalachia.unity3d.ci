using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class PackageExportsEntryOrFallbackConverter : JsonConverter
    {
        public static readonly PackageExportsEntryOrFallbackConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(PackageExportsEntryOrFallback)) ||
                   (t == typeof(PackageExportsEntryOrFallback?));
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
                    return new PackageExportsEntryOrFallback();
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new PackageExportsEntryOrFallback {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue =
                        serializer.Deserialize<PackageExportsEntryPackageExportsEntryObject>(reader);
                    return new PackageExportsEntryOrFallback
                    {
                        PackageExportsEntryPackageExportsEntryObject = objectValue
                    };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<PackageExportsEntry[]>(reader);
                    return new PackageExportsEntryOrFallback {AnythingArray = arrayValue};
            }

            throw new Exception("Cannot unmarshal type PackageExportsEntryOrFallback");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (PackageExportsEntryOrFallback) untypedValue;
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

            if (value.PackageExportsEntryPackageExportsEntryObject != null)
            {
                serializer.Serialize(writer, value.PackageExportsEntryPackageExportsEntryObject);
                return;
            }

            throw new Exception("Cannot marshal type PackageExportsEntryOrFallback");
        }
    }
}
