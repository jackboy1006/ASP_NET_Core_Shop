using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASP_NET_Core_Shop.Models;
//驗證
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
//權限
using Microsoft.AspNetCore.Authorization;

namespace ASP_NET_Core_Shop.Controllers
{
	public class ShopController : Controller
	{
		


		public IActionResult HomePage()
        {
			return View();
        }

		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(User _user)
		{
			if (!ModelState.IsValid)
			{
				TempData["Message"] = "資料未正確填寫";
				return View();
			}

			Models.Repo.UserTableRepository repo = new Models.Repo.UserTableRepository();
			bool check = repo.UserLogin(_user);

			if (!check)
			{
				TempData["Message"] = "無此帳號，請重新註冊!";
				return View();
			}

			return Content("登陸成功，HI " + _user.UserName);
		}

		public IActionResult SignUp()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SignUp(User _user)
		{
			Models.Repo.UserTableRepository repo = new Models.Repo.UserTableRepository();
			bool check = repo.AddUser(_user);
			if (!check)
			{
				TempData["Message"] = "帳號名重複，請重新填寫!";
				return View();
			}
			return Content(_user.UserName + "," + _user.Password + "," + _user.FullName + "," + _user.Email);
		}
		//權限不足
		public IActionResult AccessDeny()
		{
			return View();
		}

	}
}
