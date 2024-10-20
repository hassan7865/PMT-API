using System;
using System.Collections.Generic;

namespace Models
{
    public partial class Organization
    {
        public int OrgId { get; set; }
        public string OrgName { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
