using BnsApis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis.Converters
{
    public class GoldJsonConverter : JsonConverter<Gold>
    {
        public override Gold ReadJson(JsonReader reader, Type objectType, Gold existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            int amt = Int32.Parse(reader.Value.ToString());
            return new Gold(amt);
        }

        public override void WriteJson(JsonWriter writer, Gold value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Total);
        }
    }
}
