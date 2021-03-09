using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASP_NET_Core_Shop.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using MyDLL;

using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
		public bool AddUser(UserTable user)
		{
			IQueryable<UserTable> data = from u
									in _db.UserTables
										 where u.UserName == user.UserName || u.Email == user.Email
									select u;
			UserTable _result = data.FirstOrDefault();
			if (_result != null) return false;

			string hashStr = Account.GetSHA1Hash(user.Password);
			user.Password = hashStr;
			_db.UserTables.Add(user);
			_db.SaveChanges();
			return true;
		}

		public bool DeleteUser(UserTable user)
		{
			throw new NotImplementedException();
		}

		public IQueryable<UserTable> GetAllUsers()
		{
			throw new NotImplementedException();
		}

		public UserTable GetUserById(int id)
		{
			throw new NotImplementedException();
		}

		public UserTable GetUserByName(string name)
		{
			throw new NotImplementedException();
		}

		public UserTable UserLogin(UserTable user)
		{
			string sha256 = Account.GetSHA1Hash(user.Password);
			IQueryable<UserTable> data = from u
									in _db.UserTables
										 where u.UserName == user.UserName && u.Password == sha256
									select u;
			UserTable _result = data.FirstOrDefault();
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

        public async Task<string> DiscontinueProductAsync(int id)
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

        public async Task<string> DeleteProductAsync(int id)
        {
			var product = await _db.Products.FindAsync(id);

			if (product == null) return "此商品不存在!";

			if (!File.Exists(product.Image)) return "圖片路徑有問題";

			File.Delete(product.Image);

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return "商品已成功刪除!";
		}

		public async Task<string> AddProductToCartAsync(int userId, int id)
		{
			IQueryable<BuyCart> checkDuplicate = from b
												 in _db.BuyCarts
												 where b.UserId == userId && b.ProductId == id
												 select b;
			BuyCart existedProduct = checkDuplicate.FirstOrDefault();
			if (existedProduct != null)
			{
				existedProduct.Quantity += 1;
				_db.BuyCarts.Update(existedProduct);
				await _db.SaveChangesAsync();
				return "購物車內已存在此商品，已為您增加數量!";
			}
			IQueryable<Product> getProduct = from p
										  in _db.Products
										  where p.Id == id
										  select p;
			Product product = getProduct.FirstOrDefault();
			if (product == null) return "此商品不存在!";

			BuyCart cart = new BuyCart
			{
				ProductId = id,
				Quantity = 1,
				UserId = userId,
				ProductName = product.Name,
				Product = product
			};
			_db.BuyCarts.Add(cart);
			await _db.SaveChangesAsync();

			return "商品已成功加入購物車!";
		}

		public List<BuyCart> GetUserBuyCart(int id)
		{
			IQueryable<BuyCart> getBuyCart = from b
											 in _db.BuyCarts
											 where b.UserId == id
											 select new BuyCart 
											 { 
												UserId = b.UserId,
												ProductId = b.ProductId,
												Quantity = b.Quantity,
												ProductName = b.ProductName,
												Product = b.Product
											 };
			List<BuyCart> buyCart = getBuyCart.ToList();
			foreach (var item in buyCart)
			{
				item.Product.Image = item.Product.Image.Replace("wwwroot", "");
			}

			return buyCart;
		}

		public async Task<string> UpdateBuyCartAsync(int userId, BuyCart cart)
		{
			IQueryable<BuyCart> getBuyCart = from b
											 in _db.BuyCarts
											 where b.UserId == userId && b.ProductId == cart.ProductId
											 select b;
			BuyCart buyCart = getBuyCart.FirstOrDefault();
			if (buyCart == null) return "此商品已不存在購物車，請重新選購!";

			buyCart.Quantity = cart.Quantity;
			_db.Update(buyCart);
			await _db.SaveChangesAsync();

			return "購物車已更新";
		}

		public async Task<string> DeleteBuyCartAsync(int userId, int productId)
		{
			IQueryable<BuyCart> getBuyCart = from b
											 in _db.BuyCarts
											 where b.UserId == userId && b.ProductId == productId
											 select b;
			BuyCart buyCart = getBuyCart.FirstOrDefault();
			if(buyCart == null) return "此商品已不存在購物車!";

			_db.BuyCarts.Remove(buyCart);
			await _db.SaveChangesAsync();
			return "此商品已成功刪除!";
		}

        public string CreateOrderAsync(int userId, Order order)
        {
			//使用ADO.NET + Dapper 建立商品訂單
			var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
			IConfiguration config = configurationBuilder.Build();
			string connectionString = config["ConnectionStrings:DBConnectionString"];

			List<BuyCart> userCarts = new List<BuyCart>();

			using (SqlConnection Conn = new SqlConnection(connectionString))
			{
				Conn.Open();

				//取得訂單明細需要的資料 (購物車, 產品, 使用者)
				string sqlstr = "SELECT * FROM BuyCart b LEFT JOIN Products p On b.ProductID = p.id ";
				sqlstr += "LEFT JOIN UserTables u On b.UserID = u.id";
				var orderViewModels = Conn.Query< BuyCart, Product, UserTable, CreateOrderViewModel>(sqlstr, 
					(bt,pt,ut) => {
						CreateOrderViewModel dtEntry = new CreateOrderViewModel();
						OrderDetail orderDetail = new OrderDetail();
						orderDetail.ProductName = bt.ProductName;
						orderDetail.Quantity = bt.Quantity;
						orderDetail.ProductPrice = pt.Price;
						orderDetail.ProductImage = pt.Image;
						dtEntry.userTable = ut;
						dtEntry.orderDetail = orderDetail;
						//查詢的同時 存取此使用者的所有購物車
                        if (bt.UserId == userId)
                        {
							userCarts.Add(bt);
						}

						return dtEntry;
					}, splitOn: "id,id").ToList();

				if(userCarts.Count == 0) return "購物車已不存在請重新選購!";

				order.UserId = userId;
				order.OrderNum = DateTime.Now.ToString("yyyyMMddhhmmssfff");
				order.IsPaid = true;
				order.CreatedAt = DateTime.Now;

				//取得使用者的訂單明細
				foreach (var viewModel in orderViewModels.Where(item => item.userTable.Id == userId))
                {
					order.OrderDetails.Add(viewModel.orderDetail);
				}

				//建立訂單
				sqlstr = "INSERT INTO [Orders] ([UserID], [OrderNum], [BuyerName], [BuyerEmail], [BuyerPhone]," +
					" [ShipAddress], [ShipCity], [ShipArea], [IsPaid], [IsShipped], [IsDone], [IsCancel], [Total], [CreatedAt])";
				sqlstr += " VALUES (@UserId, @OrderNum, @BuyerName, @BuyerEmail, @BuyerPhone, @ShipAddress, @ShipCity, @ShipArea," +
					" @IsPaid, @IsShipped, @IsDone, @IsCancel, @Total, @CreatedAt);";

				var orderParameters = new
				{
					UserId = order.UserId,
					OrderNum = order.OrderNum,
					BuyerName = order.BuyerName,
					BuyerEmail = order.BuyerEmail,
					BuyerPhone = order.BuyerPhone,
					ShipAddress = order.ShipAddress,
					ShipCity = order.ShipCity,
					ShipArea = order.ShipArea,
					IsPaid = order.IsPaid,
					IsShipped = order.IsShipped,
					IsDone = order.IsDone,
					IsCancel = order.IsCancel,
					Total = order.Total,
					CreatedAt = order.CreatedAt
				};

				int affectedRows = Conn.Execute(sqlstr, orderParameters);
				//取得剛建立的訂單
				sqlstr = "SELECT * FROM [Orders] WHERE UserID = @UserId AND OrderNum = @OrderNum";
				var thisOrder = Conn.QuerySingle<Order>(sqlstr, new { UserId = userId, OrderNum = order.OrderNum });
				//設定訂單明細關聯的訂單id
                foreach (var details in order.OrderDetails)
                {
					details.OrderId = thisOrder.Id;
				}
				//建立明細
				sqlstr = "INSERT INTO [OrderDetails] ([OrderID], [ProductImage], [ProductName], [ProductPrice], [Quantity]) " +
					"VALUES (@OrderId, @ProductImage, @ProductName, @ProductPrice, @Quantity);";
				int affectedDetailRows = Conn.Execute(sqlstr, order.OrderDetails);
				//清空此使用者購物車
				sqlstr = "DELETE FROM [BuyCart] WHERE [UserID] = @UserId";
				int affectedDeleteRows = Conn.Execute(sqlstr, new { UserId = userId });
			}

            return "已完成訂單!";
		}

        public List<Order> GetUserOrders(int userId)
        {
			IQueryable<Order> getUserOrders = from o
											  in _db.Orders
											  where o.UserId == userId
											  select new Order
											  {
												  Id = o.Id,
												  OrderNum = o.OrderNum,
												  BuyerName = o.BuyerName,
												  BuyerEmail = o.BuyerEmail,
												  BuyerPhone = o.BuyerPhone,
												  ShipAddress = o.ShipAddress,
												  IsPaid = o.IsPaid,
												  IsDone = o.IsDone,
												  IsShipped = o.IsShipped,
												  IsCancel = o.IsCancel,
												  CreatedAt = o.CreatedAt,
												  Total = o.Total,
												  OrderDetails = o.OrderDetails
											  };
			List<Order> userOrders = getUserOrders.ToList();
			return userOrders;
		}

        public string CancelOrder(int userId, int orderId)
        {
			IQueryable<Order> getOrder = from o
										 in _db.Orders
										 where o.UserId == userId && o.Id == orderId
										 select o;
			Order order = getOrder.FirstOrDefault();
			if (order == null) return "此訂單已不存在! 請重新登入!";

			order.IsCancel = true;
			_db.Update(order);
			_db.SaveChanges();
			return "已完成取消申請!";
		}

        public List<Order> GetAllOrders()
        {
			IQueryable<Order> getOrders = from o
										  in _db.Orders
										  select new Order
										  {
											  Id = o.Id,
											  UserId = o.UserId,
											  OrderNum = o.OrderNum,
											  BuyerName = o.BuyerName,
											  BuyerEmail = o.BuyerEmail,
											  BuyerPhone = o.BuyerPhone,
											  ShipAddress = o.ShipAddress,
											  ShipCity = o.ShipCity,
											  IsPaid = o.IsPaid,
											  IsCancel = o.IsCancel,
											  IsShipped = o.IsShipped,
											  IsDone = o.IsDone,
											  Total = o.Total,
											  CreatedAt = o.CreatedAt,
											  OrderDetails = o.OrderDetails
										  };
			List<Order> orders = getOrders.ToList();
			return orders;
		}

        public string UpdateOrder(int id, Order order)
        {
			IQueryable<Order> getOrder = from o
										 in _db.Orders
										 where o.Id == id
										 select o;
			Order userOrder = getOrder.FirstOrDefault();
			if (userOrder == null) return "此訂單已不存在! 請重新確認!";

			userOrder.IsCancel = order.IsCancel;
			userOrder.IsDone = order.IsDone;
			userOrder.IsPaid = order.IsPaid;
			userOrder.IsShipped = order.IsShipped;
			_db.Update(userOrder);
			_db.SaveChanges();
			return "訂單完成更新!";
        }

		public SellDataViewModel GetTop10Products()
		{
			var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
			IConfiguration config = configurationBuilder.Build();
			string connectionString = config["ConnectionStrings:DBConnectionString"];

			//using (SqlConnection Conn = new SqlConnection(connectionString))
			//{
			//	Conn.Open();

			//	string sqlstr = "SELECT * FROM [OrderDetails] o " +
			//		"LEFT JOIN [Products] p ON o.ProductName = p.Name " +
			//		"LEFT JOIN [ProductTypes] pt ON p.TypeID = pt.id;";
			//	var orderViewModels = Conn.Query<BuyCart, Product, UserTable, CreateOrderViewModel>(sqlstr,
			//		(bt, pt, ut) => {
			//			CreateOrderViewModel dtEntry = new CreateOrderViewModel();
			//			OrderDetail orderDetail = new OrderDetail();
			//			orderDetail.ProductName = bt.ProductName;
			//			orderDetail.Quantity = bt.Quantity;
			//			orderDetail.ProductPrice = pt.Price;
			//			orderDetail.ProductImage = pt.Image;
			//			dtEntry.userTable = ut;
			//			dtEntry.orderDetail = orderDetail;
			//			//查詢的同時 存取此使用者的所有購物車
			//			if (bt.UserId == userId)
			//			{
			//				userCarts.Add(bt);
			//			}

			//			return dtEntry;
			//		}, splitOn: "id,id").ToList();
			//}
			//SELECT p.id,ProductName,SUM(Quantity) as Num,SUM(Quantity * ProductPrice) as Total,p.TypeID,pt.Name FROM OrderDetails o
			//LEFT JOIN Products p ON o.ProductName = p.Name
			//LEFT JOIN ProductTypes pt ON p.TypeID = pt.id
			//GROUP BY ProductName, p.TypeID ,pt.Name,p.id
			return null;
		}
	}
}
