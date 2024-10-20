using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServices.DTOS
{
    public class OrganizationUser 
    {
        public Organization Organization { get; set; }

        public User User { get; set; }
    }

    
}
