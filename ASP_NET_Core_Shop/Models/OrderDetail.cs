using System;
using System.Collections.Generic;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int Quantity { get; set; }

        public virtual Order Order { get; set; }
    }
}
