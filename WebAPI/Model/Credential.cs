using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class Credential
    {
        public required string User { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public bool Active { get; set; } = true;
    }
}
