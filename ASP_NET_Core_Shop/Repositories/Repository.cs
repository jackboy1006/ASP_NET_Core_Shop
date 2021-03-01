using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASP_NET_Core_Shop.Models;
using Microsoft.AspNetCore.Http;
using MyDLL;

namespace ASP_NET_Core_Shop.Models.Repositories
{
	public class Repository : IRepository
	{
		//連接資料庫
		private ShopDBContext _db;
		public Repository(ShopDBContext context)
		{
			_db = context;
		}
		public bool AddUser(User user)
		{
			IQueryable<User> data = from u
									in _db.Users
									where u.UserName == user.UserName || u.Email == user.Email
									select u;
			User _result = data.FirstOrDefault();
			if (_result != null) return false;

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

		public User UserLogin(User user)
		{
			string sha256 = Account.GetSHA1Hash(user.Password);
			IQueryable<User> data = from u
									in _db.Users
									where u.UserName == user.UserName && u.Password == sha256
									select u;
			User _result = data.FirstOrDefault();
			if (_result == null) return null;
			return _result;
		}

		public async Task<string> AddProductAsync(IFormCollection formData, IFormFile productimage)
        {
			IQueryable<ProductType> dbProductType = from t
													in _db.ProductTypes
													where t.Id == Convert.ToInt32(formData["type"])
													select t;
			ProductType type = dbProductType.FirstOrDefault();
			if (type == null) return "上架商品失敗! 產品類型不正確，請重新選擇!";


			IQueryable<Product> dbData = from p
									   in _db.Products
									   where p.Name == formData["name"].ToString()
										 select p;

			Product _result = dbData.FirstOrDefault();
			if (_result != null) return "上架商品失敗! 已上架相同商品!";

			bool fileOK = false;
			string path = "";
			try
			{
				if (productimage.Length > 0)
				{
					string fileName = Path.GetFileName(productimage.FileName);
					string savePath = Path.Combine("wwwroot", "Images", "Products");
					string fileExtension = System.IO.Path.GetExtension(fileName);
					string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

					for (int i = 0; i < allowedExtensions.Length; i++)
					{
						if (allowedExtensions[i] == fileExtension)
							fileOK = true;
					}
					if (!fileOK)
					{
						return "上架商品失敗! 上傳的檔案，副檔名只能是 .jpg, .jpeg, .png, .gif 這四種。";
					}
					else
					{
						string OnlyFileName = Path.GetFileNameWithoutExtension(fileName);
						string ExtFilename = Path.GetExtension(fileName);
						fileName = OnlyFileName + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ExtFilename;
						string _end = Path.Combine(savePath, fileName);
						using (var stream = System.IO.File.Create(_end))
						{
							await productimage.CopyToAsync(stream);
						}
						path = _end;

						Product product = new Product
						{
							Name = formData["name"].ToString(),
							TypeId = type.Id,
							Info = formData["info"].ToString(),
							Stock = Convert.ToInt32(formData["stock"]),
							Price = Convert.ToInt32(formData["price"]),
							Image = path,
							Active = true
						};

						_db.Products.Add(product);
                        await _db.SaveChangesAsync();

						return "商品已成功上架";
					}
				}
				else
					return "上架商品失敗! 您尚未挑選檔案，無法上傳";
			}
			catch
			{
				return "上傳失敗!!!";
			}
        }

        public List<Product> GetAllProducts()
        {
			IQueryable<Product> products = from p
										   in _db.Products
										   where p.Active == true
										   select p;
            foreach (var item in products)
            {
				item.Image = item.Image.Replace("wwwroot", "");
            }
			return products.ToList();
		}

        public List<Product> GetAllDiscontinuedProducts()
        {
			IQueryable<Product> products = from p
										   in _db.Products
										   where p.Active == false
										   select p;
			foreach (var item in products)
			{
				item.Image = item.Image.Replace("wwwroot", "");
			}
			return products.ToList();
		}

        public async Task<string> DiscontinueProduct(int id)
        {
			IQueryable<Product> product = from p
										  in _db.Products
										  where p.Id == id
										  select p;
			var _result = product.FirstOrDefault();
			if (_result == null) return "此商品不存在!";

			_result.Active = false;
			_db.Products.Update(_result);
			await _db.SaveChangesAsync();
			return "商品已成功下架!";
        }

        public async Task<string> DeleteProduct(int id)
        {
			var product = await _db.Products.FindAsync(id);

			if (product == null) return "此商品不存在!";

			if (!File.Exists(product.Image)) return "圖片路徑有問題";

			File.Delete(product.Image);

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return "商品已成功刪除!";
		}
    }
}
