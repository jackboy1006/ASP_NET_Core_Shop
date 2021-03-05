﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
		Task<string> CreateOrderAsync(int userId, Order order);
	}
}
