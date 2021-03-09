using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP_NET_Core_Shop.Models.ViewModels;
using Newtonsoft.Json;

namespace ASP_NET_Core_Shop.Models.Repositories
{
	public interface IRepository
	{
		IQueryable<UserTable> GetAllUsers();
		UserTable GetUserById(int id);
		UserTable GetUserByName(string name);
		bool AddUser(UserTable user);
		bool DeleteUser(UserTable user);
		UserTable UserLogin(UserTable user);


		Task<string> AddProductAsync(IFormCollection product, IFormFile image);
		List<Product> GetAllProducts();

		List<Product> GetAllDiscontinuedProducts();

		Task<string> DiscontinueProductAsync(int id);
		Task<string> DeleteProductAsync(int id);

		Task<string> AddProductToCartAsync(int userId,int productId);
		List<BuyCart> GetUserBuyCart(int id);
		Task<string> UpdateBuyCartAsync(int userId, BuyCart cart);
		Task<string> DeleteBuyCartAsync(int userId, int productId);
		string CreateOrderAsync(int userId, Order order);
		List<Order> GetUserOrders(int userId);
		string CancelOrder(int userId, int orderId);
		List<Order> GetAllOrders();
		string UpdateOrder(int id, Order order);

		List<SellDataViewModel> GetProductsSellData();

		DashBoardViewModel GetDashBoardData();
	}
}
