using System;
using System.Collections.Generic;

namespace Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? OrgId { get; set; }
        public int RoleId { get; set; }
    }
}
