using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_Core_Shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebAPIController : ControllerBase
	{
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
	}
}
