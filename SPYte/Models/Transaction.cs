using System;
using System.Collections.Generic;

namespace SPYte.Models
{
    public partial class Transaction
    {
        public long Id { get; set; }
        public string UserId { get; set; } = null!;
        public long OrderId { get; set; }
        public string Code { get; set; } = null!;
        public int Status { get; set; }
        public string? Type { get; set; }
        public double Total { get; set; }
        public string Unit { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
