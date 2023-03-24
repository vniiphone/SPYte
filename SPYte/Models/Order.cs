using System;
using System.Collections.Generic;

namespace SPYte.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Transactions = new HashSet<Transaction>();
        }

        public long Id { get; set; }
        public double OrderTotal { get; set; }
        public double OrderGrandTotal { get; set; }
        public int? OrderStatus { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
