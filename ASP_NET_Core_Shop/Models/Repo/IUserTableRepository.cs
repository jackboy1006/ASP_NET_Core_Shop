﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP_NET_Core_Shop.Models;

namespace ASP_NET_Core_Shop.Models.Repo
{
	interface IUserTableRepository
	{
		IQueryable<User> GetAllUsers();
		User GetUserById(int id);
		User GetUserByName(string name);
		bool AddUser(User user);
		bool DeleteUser(User user);
		bool UserLogin(User user);

	}
}
