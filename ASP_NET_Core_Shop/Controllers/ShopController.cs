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

		#region Account
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(UserTable _user)
		{
			if (!ModelState.IsValid)
			{
				TempData["Message"] = "資料未正確填寫";
				return View();
			}

			UserTable userData = _repository.UserLogin(_user);

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

			if (userData.IsAdmin) return RedirectToAction("AdminHomePage");
			return RedirectToAction("HomePage");
			//return Content("登入成功");
		}
		public IActionResult SignUp()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SignUp(UserTable _user)
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
			//return Content("已登出");
			return Redirect("HomePage");   // 登出，跳去另一頁。
		}

		#endregion

		#region Customer

		public IActionResult HomePage()
		{
			var _result = _repository.GetAllProducts();
			return View(_result);
		}

		public IActionResult Products()
        {
			var _result = _repository.GetAllProducts();
			return View(_result);
		}

		[Authorize]
		public IActionResult BuyCart()
		{
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return RedirectToAction("Login");
			}

			var result = _repository.GetUserBuyCart(Convert.ToInt32(userId));

			return View(result);
		}

		[Authorize]
		[HttpPost]
		public IActionResult AddProductToBuyCart(int id)
		{
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return StatusCode(401);
			}

			var _result = _repository.AddProductToCartAsync(Convert.ToInt32(userId), id);


			return Ok(_result.Result);
		}

		[HttpPost]
		public IActionResult UpdateBuyCart([FromBody]BuyCart cart)
		{
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return StatusCode(401);
			}
			var result = _repository.UpdateBuyCartAsync(Convert.ToInt32(userId), cart);
			return Ok(result.Result);
		}

		[HttpPost]
		public IActionResult DeleteBuyCart(int id)
		{
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return StatusCode(401);
			}
			var result = _repository.DeleteBuyCartAsync(Convert.ToInt32(userId), id);
			return Ok(result.Result);
		}

		[Authorize]
		public IActionResult CheckOut(string discount)
		{
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return StatusCode(401);
			}

			if (discount == "usecoupon") ViewData["usecoupon"] = 50;

			var result = _repository.GetUserBuyCart(Convert.ToInt32(userId));

			return View(result);
		}
		[HttpPost]
		public IActionResult CreateOrder([FromBody]Order order)
        {
            string userId = "";
            ClaimsPrincipal principal = HttpContext.User;
            if (principal != null)
            {
                foreach (Claim claim in principal.Claims)
                {
                    if (claim.Type == "User_ID")
                    {
                        userId = claim.Value;
                    }
                }
            }
            else
            {
                return StatusCode(401);
            }

            var result = _repository.CreateOrderAsync(Convert.ToInt32(userId), order);
            return Ok(result);
        }
		[Authorize]
		public IActionResult Order()
        {
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return StatusCode(401);
			}

			var orders = _repository.GetUserOrders(Convert.ToInt32(userId));

			return View(orders);
		}
		[HttpPost]
		public IActionResult CancelOrder(int id)
        {
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return StatusCode(401);
			}

			var result = _repository.CancelOrder(Convert.ToInt32(userId), id);

			return Ok(result);
        }

		#endregion

		#region Administrator

		[Authorize(Roles = "Admin")]
		public IActionResult AdminHomePage()//等產品新增功能及頁面完成 要再處理此頁面的產品資料
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		public IActionResult AdminCreateProduct()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		public IActionResult AdminListProducts()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		public IActionResult AdminDiscontinuedProducts()
		{
			return View();
		}
		[Authorize(Roles = "Admin")]
		public IActionResult AdminOrders()
        {
			return View();
        }

		#endregion

		#region ErrorHandle
		//權限不足
		public IActionResult AccessDeny()
		{
			return View();
		}
		#endregion

		#region Test
		public IActionResult Testsql()
        {
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			else
			{
				return StatusCode(401);
			}
			Order empty = new Order();

			return View(_repository.CreateOrderAsync(Convert.ToInt32(userId), empty));
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

		public IActionResult TestCreate()
        {
			return View();
        }

		public IActionResult TestList()
		{
			var _result = _repository.GetProductsSellData();
			return View(_result);
		}

		public IActionResult TestListBuyCart()
		{
			string userId = "";
			ClaimsPrincipal principal = HttpContext.User;
			if (principal != null)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == "User_ID")
					{
						userId = claim.Value;
					}
				}
			}
			return View(_repository.GetUserBuyCart(Convert.ToInt32(userId)));
		}
		#endregion

	}
}
