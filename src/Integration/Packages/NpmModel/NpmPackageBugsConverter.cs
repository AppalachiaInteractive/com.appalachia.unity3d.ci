using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class NpmPackageBugsConverter : JsonConverter
    {
        #region Constants and Static Readonly

        public static readonly NpmPackageBugsConverter Singleton = new();

        #endregion

        public override bool CanConvert(Type t)
        {
            return (t == typeof(NpmPackageBugs)) || (t == typeof(NpmPackageBugs?));
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
                    return new NpmPackageBugs {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PurpleBugs>(reader);
                    return new NpmPackageBugs {PurpleBugs = objectValue};
            }

            throw new Exception("Cannot unmarshal type NpmPackageBugs");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (NpmPackageBugs) untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }

            if (value.PurpleBugs != null)
            {
                serializer.Serialize(writer, value.PurpleBugs);
                return;
            }

            throw new Exception("Cannot marshal type NpmPackageBugs");
        }
    }
}
