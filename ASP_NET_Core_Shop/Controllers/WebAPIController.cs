using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASP_NET_Core_Shop.Models;
using System.IO;
using ASP_NET_Core_Shop.Models.Repositories;
using ASP_NET_Core_Shop.Models.ViewModels;
using Newtonsoft.Json;

namespace ASP_NET_Core_Shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebAPIController : ControllerBase
	{
		private readonly IRepository _repository;
		public WebAPIController(IRepository repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// 查詢已登入的使用者名稱
		/// </summary>
		/// <returns>使用者名稱</returns>
		[HttpGet]
		[Route("GetUserName")]
		public string GetUserName()
		{
			string name = "";
			ClaimsPrincipal principal = (ClaimsPrincipal)HttpContext.User;
			if (null != principal)
			{
				foreach (Claim claim in principal.Claims)
				{
					if (claim.Type == ClaimTypes.Name)
						name = claim.Value;
				}
			}
			if (name != "") return name;
			return null;
		}
		/// <summary>
		/// 新增產品
		/// </summary>
		/// <param name="data">產品資訊表單</param>
		/// <param name="productimage">產品圖片</param>
		/// <returns>結果訊息</returns>
		/// <response code="200">回傳結果訊息</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult AddProduct(IFormCollection data, IFormFile productimage)
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

			Task<string> str = _repository.AddProductAsync(data, productimage);
			var result = new
			{
				message = str.Result,
			};
			return Ok(result);
		}
		/// <summary>
		/// 查詢所有上架產品
		/// </summary>
		/// <returns>所有產品</returns>
		/// <response code="200">回傳所有上架產品</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpGet]
		[Route("GetAllProducts")]
		[ProducesResponseType(typeof(List<Product>), 200)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult GetAllProducts()
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

			var result = _repository.GetAllProducts();
			return Ok(result);
		}
		/// <summary>
		/// 查詢所有下架產品
		/// </summary>
		/// <returns></returns>
		/// <response code="200">回傳所有下架產品</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpGet]
		[Route("GetAllDiscontinuedProducts")]
		[ProducesResponseType(typeof(List<Product>), 200)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult GetAllDiscontinuedProducts()
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

			var result = _repository.GetAllDiscontinuedProducts();
			return Ok(result);
		}
		/// <summary>
		/// 下架產品
		/// </summary>
		/// <param name="id"></param>
		/// <returns>結果訊息</returns>
		/// <response code="200">回傳結果訊息</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpPost]
		[Route("DiscontinueProduct/{id}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult PostDiscontinueProduct(int id)
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

			var str = _repository.DiscontinueProductAsync(id);
			var result = new
			{
				message = str.Result,
			};
			return Ok(result);
		}
		/// <summary>
		/// 刪除產品
		/// </summary>
		/// <param name="id"></param>
		/// <returns>結果訊息</returns>
		/// <response code="200">回傳結果訊息</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpDelete]
		[Route("Product/{id}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult DeleteProduct(int id)
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

			var str = _repository.DeleteProductAsync(id);
			var result = new
			{
				message = str.Result,
			};
			return Ok(result);
		}
		/// <summary>
		/// 查詢所有訂單
		/// </summary>
		/// <returns></returns>
		/// <response code="200">回傳所有訂單</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpPost]
		[Route("GetAllOrders")]
		[ProducesResponseType(typeof(List<Order>), 200)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult GetAllOrders()
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

			var result = _repository.GetAllOrders();
			return Ok(result);
		}
		/// <summary>
		/// 查詢特定城市所有訂單
		/// </summary>
		/// <param name="city"></param>
		/// <returns></returns>
		/// <response code="200">回傳城市所有訂單</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpPost]
		[Route("GetAllOrders/{city}")]
		[ProducesResponseType(typeof(List<Order>), 200)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult GetAllOrders(string city)
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

			var result = _repository.GetAllOrders(city);
			return Ok(result);
		}
		/// <summary>
		/// 更新訂單狀態
		/// </summary>
		/// <param name="id"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		/// <response code="200">回傳結果訊息</response>
		/// <response code="401">登入驗證失敗</response>
		[HttpPatch]
		[Route("Order/{id}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public IActionResult UpdateOrder(int id, Order order)
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

			var result = _repository.UpdateOrder(id, order);

			return Ok(result);
        }
		/// <summary>
		/// 查詢所有銷售統計
		/// </summary>
		/// <returns></returns>
		/// <response code="200">
		/// 回傳產品銷售資訊 
		/// <remarks>
		/// 
		/// Response ViewModel:
		///
		///     [
		///		  {
		///			 "id": 1,
		///			 "ProductName": "杯子蛋糕",
		///			 "Quantity": 10,
		///			 "Total": 2900,
		///			 "ProductType": "蛋糕"
		///		  }
		///     ]
		///
		/// </remarks>
		/// </response>
		/// <response code="401">登入驗證失敗</response>
		[HttpGet]
		[Route("GetProductsSellData")]
		public IActionResult GetProductsSellData()
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
			var result = _repository.GetProductsSellData();

			return Ok(JsonConvert.SerializeObject(result));
		}
		/// <summary>
		/// 查詢週統計資料
		/// </summary>
		/// <returns></returns>
		/// <response code="200">
		/// 回傳週統計資料
		/// <remarks>
		/// 
		/// Response ViewModel:
		///
		///	{
		///		"SundayTotal": 1000,
		///		"MondayTotal": 2000,
		///		"TuesdayTotal": 3000,
		///		"WednesdayTotal": 4000,
		///		"ThursdayTotal": 5000,
		///		"FridayTotal": 6000,
		///		"SaturdayTotal": 7000
		///	}
		///
		/// </remarks>
		/// </response>
		/// <response code="401">登入驗證失敗</response>
		[HttpGet]
		[Route("GetDashBoardData")]
		public IActionResult GetDashBoardData()
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

			var result = _repository.GetDashBoardData();

            return Ok(JsonConvert.SerializeObject(result));
		}

	}
}
