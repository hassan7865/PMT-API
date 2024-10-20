using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServices.DTOS
{
    public class JwtDTO
    {
        public string Key { get; set; } 

        public string Issuer { get; set; }
    }
}
