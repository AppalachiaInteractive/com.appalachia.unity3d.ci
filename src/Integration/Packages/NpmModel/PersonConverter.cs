using System;
using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    internal class PersonConverter : JsonConverter
    {
        public static readonly PersonConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return (t == typeof(Person)) || (t == typeof(Person?));
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
                    return new Person {String = stringValue};
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<PersonClass>(reader);
                    return new Person {PersonClass = objectValue};
            }

            throw new Exception("Cannot unmarshal type Person");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Person) untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }

            if (value.PersonClass != null)
            {
                serializer.Serialize(writer, value.PersonClass);
                return;
            }

            throw new Exception("Cannot marshal type Person");
        }
    }
}
