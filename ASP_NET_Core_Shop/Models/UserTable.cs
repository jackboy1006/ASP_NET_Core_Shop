using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class UserTable
    {
        public UserTable()
        {
            Orders = new HashSet<Order>();
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="請輸入使用者名稱!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "請輸入使用者密碼")]
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool UserApproved { get; set; }
        public bool IsAdmin { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
