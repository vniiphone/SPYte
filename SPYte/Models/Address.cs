using System;
using System.Collections.Generic;

namespace SPYte.Models
{
    public partial class Address
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? WardId { get; set; }
        public string? Location { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
        public virtual Ward? Ward { get; set; }
    }
}
