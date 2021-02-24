﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyDLL;

namespace ASP_NET_Core_Shop.Models.Repo
{
	public class UserTableRepository : IUserTableRepository
	{
		//連接資料庫
		private readonly ShopDBContext _db;

		public UserTableRepository()
		{
		}

		public UserTableRepository(ShopDBContext context)
		{
			_db = context;
		}
		public bool AddUser(User user)
		{
			IQueryable<User> data = from u
									in _db.Users
									where u.UserName == user.UserName || u.Email == user.Email
									select u;
			if (data != null)
				return false;

			string hashStr = Account.GetSHA1Hash(user.Password);
			user.Password = hashStr;
			_db.Users.Add(user);
			_db.SaveChanges();
			return true;
		}

		public bool DeleteUser(User user)
		{
			throw new NotImplementedException();
		}

		public IQueryable<User> GetAllUsers()
		{
			throw new NotImplementedException();
		}

		public User GetUserById(int id)
		{
			throw new NotImplementedException();
		}

		public User GetUserByName(string name)
		{
			throw new NotImplementedException();
		}

		public bool UserLogin(User user)
		{
			string sha256 = Account.GetSHA1Hash(user.Password);
			IQueryable<User> data = from u
									in _db.Users
									where u.UserName == user.UserName && u.Password == sha256
									select u;
			User _result = data.FirstOrDefault();
			if (_result == null) return false;
			return true;
		}

	}
}
