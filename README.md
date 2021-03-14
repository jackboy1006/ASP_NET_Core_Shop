# ASP_NET_Core_Shop 購物網站
## 簡介
### 前台顧客區，使用 前端(JQuery/JS) + 後端(MVC)
實作功能 (會員註冊登入、產品清單、購物車、訂單CRUD)

### 後臺管理區，使用 前端(JQuery/JS) + 後端(WebAPI)
實作功能 (Dashboard週報表、商品銷售報表、商品CRUD、訂單處理)

後臺管理帳戶: admin

後臺管理密碼: admin

### Azure 部署
App Service, SQL Server, SQL DB
## 導覽功能
* [登入及註冊](#login)
* [商品清單](#products)
* [購物車](#cart)
* [顧客區訂單](#order)
* [Dashboard週報表、商品銷售報表](#dash)
* [商品銷售報表](#sell)
* [商品CRUD](#adminProducts)
* [管理區訂單](#adminOrder)

<h2 id="demo"> 網站Demo網址 </h2>
https://coreshopdemo.azurewebsites.net/Shop/HomePage

## 後臺管理區 Open API
https://coreshopdemo.azurewebsites.net/swagger

## 資料庫關係圖
![CoreShopDB](https://user-images.githubusercontent.com/46698894/111060129-ac62f900-84d5-11eb-830c-918b1afba26b.png)

<h2 id="func"> 主要功能 </h2>

<h3 id="login"> 登入及註冊 </h3>

* 使用 ClaimsIdentity 進行驗證
* 密碼以雜湊進行加密
* 會員權限區分，依照資料表 **UserTables.IsAdmin** bool值，給予票證 "Admin" 或 "Customer"
* "Admin" 登入後導向後台管理頁面，"Customer" 登入後導向顧客區頁面
* 程式碼位置 : Controllers => ShopController => Login、SignUp
* 程式碼位置 : MyDLL => Account => GetSHA1Hash

![SignUp](https://user-images.githubusercontent.com/46698894/111064013-bf34f800-84ec-11eb-8545-a41bd5209723.png)
![HomePage](https://user-images.githubusercontent.com/46698894/111064023-ce1baa80-84ec-11eb-81d3-b145dc9d0177.png)
![adminHome](https://user-images.githubusercontent.com/46698894/111064027-d4118b80-84ec-11eb-9cdf-aadd034b6bd6.png)


<h3 id="products"> 商品清單 </h3>

* 回傳所有商品清單
* 前端依照商品種類(Product.TypeID)進行分類顯示

![商品區](https://user-images.githubusercontent.com/46698894/111064076-0327fd00-84ed-11eb-9af2-05718c2c450f.png)


<h3 id="cart"> 購物車 </h3>

* 依照每項商品建立**BuyCart**紀錄，一個**BuyCart**對應一個**Product**，紀錄該使用者購物車內此商品資訊(數量)
* 更新購物車，依照前端數量更改，即時更改資料庫數據
* 顯示購物車，查詢出所有該使用者的**BuyCart**，回傳
* 優惠券使用，前端即時運算顯示金額
* 程式碼位置 : Controllers => ShopController => BuyCart、AddProductToBuyCart、UpdateBuyCart、DeleteBuyCart

![購物車](https://user-images.githubusercontent.com/46698894/111064100-2c488d80-84ed-11eb-9416-fd56d2a61d85.png)

<h3 id="order"> 顧客區訂單 </h3>

* 前端表單驗證必填項目
* 依照購物車所有商品進行結帳，並取得是否使用優惠券(參數)
* **使用ADO.NET + Dapper** 查詢所有對應商品，建立**OrderDetails**及此筆訂單**Order**
* 顯示訂單，回傳該使用者訂單(包含OrderDetails)，由前端處理主細表顯示
* 顧客可以查看管理者更改的訂單狀態，也可提出取消訂單申請
* 程式碼位置 : Controllers => ShopController => CheckOut、CreateOrder、Order、CancelOrder

![結帳](https://user-images.githubusercontent.com/46698894/111064158-73368300-84ed-11eb-8e75-2a8b815edb98.png)
![顧客訂單](https://user-images.githubusercontent.com/46698894/111064505-54d18700-84ef-11eb-9bc3-539e1aff9cd9.png)


<h3 id="dash"> Dashboard、商品銷售報表 </h3>

#### Dashboard

* 查詢所有訂單分別為星期幾，並加總訂單金額，存進**DashBoardViewModel**
* 前端依照數據顯示Dashboard週報表趨勢圖
* 程式碼位置 : Controllers => WebAPIController => GetDashBoardData


#### 商品銷售報表
* **使用ADO.NET + Dapper** 查詢所有並排序**OrderDetails**(訂單明細)，JOIN **Products**、**ProductTypes** 存進 **SellDataViewModel**
* 程式碼位置 : Controllers => WebAPIController => GetProductsSellData

![報表](https://user-images.githubusercontent.com/46698894/111064340-72eab780-84ee-11eb-92eb-3948fdaf98a4.png)

<h3 id="adminProducts"> 商品CRUD </h3>

* 新增商品，商品圖片存Server硬碟內並給予流水號，資料庫只存取路徑字串
* 管理者可以查看上架或已下架商品，也可以下架及刪除商品
* 程式碼位置 : Controllers => WebAPIController => AddProduct、GetAllProducts、GetAllDiscontinuedProducts、PostDiscontinueProduct、DeleteProduct

![上架商品](https://user-images.githubusercontent.com/46698894/111064386-c1985180-84ee-11eb-8a4a-486e4a73b4c0.png)
![商品管理](https://user-images.githubusercontent.com/46698894/111064388-c3621500-84ee-11eb-9379-2061867253df.png)
![已下架商品](https://user-images.githubusercontent.com/46698894/111064387-c2c97e80-84ee-11eb-8476-c3656b5a4bb5.png)


<h3 id="adminOrder"> 管理區訂單 </h3>

* 顯示所有使用者訂單
* 更改訂單狀態(已出貨、已取消)，更改後顧客查看狀態即更改
* 程式碼位置 : Controllers => WebAPIController => GetAllOrders、GetAllOrders(string city)、UpdateOrder

![訂單管理](https://user-images.githubusercontent.com/46698894/111064449-0ae8a100-84ef-11eb-8a4e-8b6ab26a15ec.png)


