using System;
using System.Collections.Generic;

namespace SPYte.Models
{
    public partial class Cart
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public long ProductId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public double TotalPrice { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
