using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiProject.Models
{
    public class SignUpModel
    {
        public string UserName { get; set; }
        public string HashedUserNameAndPassword { get; set; }
    }
}
