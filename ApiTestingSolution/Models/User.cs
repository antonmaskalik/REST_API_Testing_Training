using ApiTestingSolution.Enums;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ApiTestingSolution.Models
{
    public class User : ICloneable
    {
        [JsonProperty("age")]
        public int? Age { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sex")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Sex Sex { get; set; }
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            User other = (User)obj;

            return Age == other.Age &&
                   Name == other.Name &&
                   Sex == other.Sex &&
                   ZipCode == other.ZipCode;
        }

        public override int GetHashCode() => Age.GetHashCode() * Name.GetHashCode();

        public override string ToString()
        {
            return $"Age: '{Age}', " +
                $"Name: '{Name}', " +
                $"Sex: '{Sex}', " +
                $"ZipCode: '{ZipCode}'";
        }

        public object Clone()
        {
            return new User
            {
                Age = Age,
                Name = Name,
                Sex = Sex,
                ZipCode = ZipCode
            };
        }
    }
}
