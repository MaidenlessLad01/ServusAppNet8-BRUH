using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServusAppNet8.MVVM.Models
{
    public class User
    {
        //var dec
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string PhoneNum { get; set; }
        public DateTime DoB { get; set; }
        public List<string> Gender { get; set; }
        public string SelectedGender { get; set; }  
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        //gender list
        public User()
        {
            Gender = new List<String> { "Male", "Female", "Shopping Cart", "Godzilla", "Walmart Bag", "Attack Helicopter", "Prefer Not To Say" };
        }
    }
}
