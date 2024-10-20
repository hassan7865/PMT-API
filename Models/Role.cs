using System;
using System.Collections.Generic;

namespace Models
{
    public partial class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
