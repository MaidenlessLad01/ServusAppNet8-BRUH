using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServusAppNet8.MVVM.Models
{
    //Model for the post on home that needs information from both User and Post
    public class PostUser
    {
        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public DateTime PostDate { get; set; }
        public string? Caption { get; set; }
        public string? Picture { get; set; }
        public string PostId { get; set; }
        public required bool SameUser { get; set; }
    }
}
