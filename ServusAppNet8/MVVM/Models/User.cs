using ServusAppNet8.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServusAppNet8.MVVM.Models
{
    public class User
    {
        //var dec
        public string UserId { get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string PhoneNum { get; set; }

        //Converts the DateTime data to be readable for both the system and the API
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DoB { get; set; }

        public string Gender { get; set; }  
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
