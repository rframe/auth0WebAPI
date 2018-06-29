using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth0WebAPI.Models
{
    public class Claim
    {
        public string Type;
        public string Value;
        public Claim(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
