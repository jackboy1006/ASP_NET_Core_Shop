using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string OrderNum { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipArea { get; set; }
        public bool IsPaid { get; set; }
        public bool IsShipped { get; set; }
        public bool IsDone { get; set; }
        public bool IsCancel { get; set; }
        public int Total { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual UserTable User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
