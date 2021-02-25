using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASP_NET_Core_Shop.Models.Repositories;
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
		private readonly IRepository _repository;
		public ShopController(IRepository repository)
        {
			_repository = repository;
        }


		public IActionResult HomePage()
        {
			return View();
        }
		[Authorize(Roles = "Admin")]
		public IActionResult AdminPage()
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

			User userData = _repository.UserLogin(_user);

			if (userData == null)
			{
				TempData["Message"] = "無此帳號，請重新註冊!";
				return View();
			}
			string role;
			if (userData.IsAdmin) role = "Admin";
			else role = "Customer";

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name,userData.UserName),
				new Claim("User_ID",userData.Id.ToString()),
				new Claim(ClaimTypes.Role, role)
			};

			ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

			AuthenticationProperties authProps = new AuthenticationProperties
			{
				AllowRefresh = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
				IsPersistent = true,
			};
			HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProps);

			if (userData.IsAdmin) return RedirectToAction("AdminPage");
			return RedirectToAction("HomePage");
			//return Content("登入成功");
		}

		public IActionResult SignUp()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SignUp(User _user)
		{
			bool check = _repository.AddUser(_user);
			if (!check)
			{
				TempData["Message"] = "帳號名重複，請重新填寫!";
				return View();
			}
			TempData["Message"] = "註冊成功! 請進行登入~";
			//return Content(_user.UserName + "," + _user.Password + "," + _user.FullName + "," + _user.Email);
			return RedirectToAction("Login");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			ViewData.Clear();
			return Content("已登出");
			//return RedirectToPage("/Account/SignedOut");   // 登出，跳去另一頁。
		}

		//權限不足
		public IActionResult AccessDeny()
		{
			return View();
		}


		[Authorize]
		public IActionResult TestLogin()
		{
			return View();
		}
		[Authorize(Roles = "Admin")]
		public IActionResult TestAdminLogin()
		{
			return View();
		}

	}
}
