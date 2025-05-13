using ServusAppNet8.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServusAppNet8.MVVM.Models
{
    public class Post
    {
        //var declaration
        public string PostId { get; set; }
        public string? Caption { get; set; }
        public string? Picture { get; set; }
        public string UserId { get; set; }

        //Converts the DateTime data to be readable for both the system and the API
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime PostDate { get; set; }
    }
}
