using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class Credential
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; } = true;
    }
}
