using System;
using System.Collections.Generic;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class BuyCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public string ProductName { get; set; }

        public virtual Product Product { get; set; }
    }
}
