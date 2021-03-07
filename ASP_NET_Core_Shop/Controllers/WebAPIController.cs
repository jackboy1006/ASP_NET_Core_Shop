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
			//return "測試";
		}
        [HttpPost]
        public IActionResult AddProduct(IFormCollection data, IFormFile productimage)
        {
            Task<string> str = _repository.AddProductAsync(data, productimage);
            var result = new
            {
                message = str.Result,
            };
            //Product product = new Product
            //{
            //    TypeId = Convert.ToInt32(data["type"]),
            //    Name = data["name"],
            //    Info = data["info"],
            //    Stock = Convert.ToInt32(data["stock"]),
            //    Price = Convert.ToInt32(data["price"]),
            //    Image = productimage.FileName,
            //    Active = true
            //};
            return Ok(result);
        }
		[HttpGet]
		[Route("GetAllProducts")]
		public IActionResult GetAllProducts()
        {
			var result = _repository.GetAllProducts();
			return Ok(result);
        }

		[HttpGet]
		[Route("GetAllDiscontinuedProducts")]
		public IActionResult GetAllDiscontinuedProducts()
		{
			var result = _repository.GetAllDiscontinuedProducts();
			return Ok(result);
		}

		[HttpPost]
		[Route("DiscontinueProduct/{id}")]
		public IActionResult PostDiscontinueProduct(int id)
		{
			var str = _repository.DiscontinueProductAsync(id);
			var result = new
			{
				message = str.Result,
			};
			return Ok(result);
		}

		[HttpDelete]
		[Route("Product/{id}")]
		public IActionResult DeleteProduct(int id)
		{
			var str = _repository.DeleteProductAsync(id);
			var result = new
			{
				message = str.Result,
			};
			return Ok(result);
		}
		[HttpPost]
		[Route("GetAllOrders")]
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
		[HttpPatch]
		[Route("Order/{id}")]
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

	}
}
