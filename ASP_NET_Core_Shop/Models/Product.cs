using System;
using System.Collections.Generic;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Info { get; set; }
        public int Stock { get; set; }
        public int Price { get; set; }
        public bool Active { get; set; }

        public virtual ProductType Type { get; set; }
    }
}
