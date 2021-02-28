using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ASP_NET_Core_Shop.Models
{
    public partial class Product
    {
        [key]
        public int Id { get; set; }
        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name = "產品種類")]
        public int TypeId { get; set; }
        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name = "產品名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name = "產品圖片")]
        public string Image { get; set; }

        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name = "產品敘述")]
        public string Info { get; set; }

        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name = "產品庫存")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "此欄位為必填 請輸入!")]
        [Display(Name = "產品價格")]
        public int Price { get; set; }
        public bool Active { get; set; }

        public virtual ProductType Type { get; set; }
    }
}
