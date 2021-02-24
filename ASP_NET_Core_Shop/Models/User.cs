using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }
        [key]
        public int Id { get; set; }
        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name = "帳號")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name ="密碼")]
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool UserApproved { get; set; }
        public bool IsAdmin { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
