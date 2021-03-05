﻿using System;
using System.Collections.Generic;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class UserTable
    {
        public UserTable()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool UserApproved { get; set; }
        public bool IsAdmin { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}