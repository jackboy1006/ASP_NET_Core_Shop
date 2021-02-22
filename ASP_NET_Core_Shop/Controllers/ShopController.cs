﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASP_NET_Core_Shop.Models;

namespace ASP_NET_Core_Shop.Controllers
{
	public class ShopController : Controller
	{
		//連接資料庫
		private readonly ShopDBContext _db;
		public ShopController(ShopDBContext context)
        {
			_db = context;
        }


		public IActionResult HomePage()
        {
			return View();
        }

	}
}
