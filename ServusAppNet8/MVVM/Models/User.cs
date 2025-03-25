using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServusAppNet8.MVVM.Models
{
    public class User
    {
        public string FName {  get; set; }
        public string LName { get; set; }
        public string PhoneNum { get; set; }
        public DateOnly DoB { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
