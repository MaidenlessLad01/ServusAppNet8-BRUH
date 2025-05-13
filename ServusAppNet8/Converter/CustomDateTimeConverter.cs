using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServusAppNet8.Converter
{
    class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        //the date format
        private readonly string format = "dd/MM/yyyy";

        //Reads the date from the api for C#
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), format, null);
        }

        //Writes the date for the api
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(format));
        }
    }
}