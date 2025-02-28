using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SonicStore.Areas.SonicStore.Dtos;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers
{
    [Area("SonicStore")]
    [Route("orders/[action]")]
    public class OrdersController : Controller
    {
        private readonly SonicStoreContext _storeContext;

        public OrdersController(SonicStoreContext context)
        {
            _storeContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int tabIndex = 1, string keyword = "")
        {      // Retrieve user information from session
            var userJson = HttpContext.Session.GetString("user");
            // Deserialize JSON string to User object
            var user = JsonConvert.DeserializeObject<User>(userJson ?? "");

            var userId = user?.Id ?? 0;
            var orders = new List<OrderModel>();

            var dbIndexes = await _storeContext.Orders.Select(x => x.index).ToListAsync();

            var indexes = dbIndexes.DistinctBy(x => x).ToList();

            var dbOrders = new Dictionary<int, List<Checkout>>();

            foreach (var index in indexes)
            {
                var listOrderByIndex = await _storeContext.Orders.Include(x => x.OrderDetails).Where(x => x.index == index).ToListAsync();
                dbOrders.Add(index, listOrderByIndex);
            }

            foreach (var key in dbOrders.Keys)
            {
                var listOrderByIndex = dbOrders[key];
                var orderDetails = new List<OrderDetailsModel>();
                foreach (var order in listOrderByIndex)
                {
                    var dbOrderDetails = order.OrderDetails;
                    var cartProduct = await (from storage in _storeContext.Storages
                                             join product in _storeContext.Products on storage.ProductId equals product.Id
                                             where storage.Id == dbOrderDetails.StorageId
                                             select new
                                             {
                                                 productId = product.Id,
                                                 imageUrl = product.Image,
                                                 productName = product.Name,
                                                 price = storage.SalePrice
                                             }).FirstOrDefaultAsync();

                    orderDetails.Add(new OrderDetailsModel
                    {
                        ProductId = cartProduct?.productId,
                        OrderDetailsId = dbOrderDetails.Id,
                        ProductName = cartProduct?.productName,
                        ImageUrl = cartProduct?.imageUrl,
                        Price = cartProduct?.price,
                        Quantity = dbOrderDetails.Quantity ?? 0,
                        Total = cartProduct?.price * (dbOrderDetails.Quantity ?? 0),
                        CustomerId = dbOrderDetails.CustomerId
                    });

                }

                var firstOrder = listOrderByIndex.FirstOrDefault();
                var statusOrder = await _storeContext.StatusOrders.FirstOrDefaultAsync(x => x.OrderId == firstOrder.Id);

                var anyOrder = orderDetails.Where(x => x.CustomerId == userId).Any();
                if (anyOrder)
                {
                    orders.Add(new OrderModel()
                    {
                        OrderDate = firstOrder?.OrderDate,
                        OrderId = firstOrder?.Id,
                        Status = statusOrder?.Type,
                        OrderDetails = orderDetails,
                        Total = orderDetails.Sum(x => x.Total),
                        Index = key,
                    });
                }
            }


            ViewBag.Orders = orders
                .Where(x => keyword.Contains((x.OrderId ?? 0).ToString())
                || x.OrderDetails.Any(x => (x.ProductName ?? "").Contains(keyword)))
                .ToList();

            ViewBag.TabIndex = tabIndex;
            ViewBag.KeyWord = keyword;
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userJson = HttpContext.Session.GetString("user");

            // Deserialize JSON string to User object
            var user = JsonConvert.DeserializeObject<User>(userJson ?? "");

            var userId = user?.Id ?? 0;
            var listOrderByIndex = await _storeContext.Orders.Include(x => x.OrderDetails).Where(x => x.index == id).ToListAsync();

            var orderDetails = new List<OrderDetailsModel>();
            foreach (var order in listOrderByIndex)
            {
                var dbOrderDetails = order.OrderDetails;
                var cartProduct = await (from storage in _storeContext.Storages
                                         join product in _storeContext.Products on storage.ProductId equals product.Id
                                         where storage.Id == dbOrderDetails.StorageId
                                         select new
                                         {
                                             productId = product.Id,
                                             imageUrl = product.Image,
                                             productName = product.Name,
                                             price = storage.SalePrice
                                         }).FirstOrDefaultAsync();

                orderDetails.Add(new OrderDetailsModel
                {
                    ProductId = cartProduct?.productId,
                    OrderDetailsId = dbOrderDetails.Id,
                    ProductName = cartProduct?.productName,
                    ImageUrl = cartProduct?.imageUrl,
                    Price = cartProduct?.price,
                    Quantity = dbOrderDetails.Quantity ?? 0,
                    Total = cartProduct?.price * (dbOrderDetails.Quantity ?? 0),
                });

            }

            var firstOrder = listOrderByIndex.FirstOrDefault();
            var statusOrder = await _storeContext.StatusOrders.FirstOrDefaultAsync(x => x.OrderId == firstOrder.Id);

            ViewBag.Order = new OrderModel()
            {
                OrderDate = firstOrder?.OrderDate,
                OrderId = firstOrder?.Id,
                Status = statusOrder?.Type,
                OrderDetails = orderDetails,
                Total = orderDetails.Sum(x => x.Total),
                Index = id,
            };

            ViewBag.Address = await _storeContext.UserAddresses
                .Where(u => u.UserId == userId && u.Status == true)
                .Select(u => u.User_Address)
                .FirstOrDefaultAsync();

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ReOrder(int id)
        {
            var userJson = HttpContext.Session.GetString("user");

            // Deserialize JSON string to User object
            var user = JsonConvert.DeserializeObject<User>(userJson ?? "");

            var userId = user?.Id ?? 0;
            var listOrderByIndex = await _storeContext.Orders.Include(x => x.OrderDetails).Where(x => x.index == id).ToListAsync();

            var orderDetails = new List<OrderDetailsModel>();
            var index = 0;
            foreach (var order in listOrderByIndex)
            {
                var dbOrderDetails = order.OrderDetails;

                _storeContext.Payments.Add(
                     new Payment()
                     {
                         TotalPrice = listOrderByIndex.Sum(x => x.OrderDetails.Price * x.OrderDetails.Quantity),
                         PaymentMethod = "credit card",
                         TransactionDate = DateTime.Now,
                     });
                await _storeContext.SaveChangesAsync();

                _storeContext.OrderDetails.Add(
                   new Cart()
                   {
                       StorageId = dbOrderDetails?.StorageId ?? 0,
                       Status = "bill",
                       CustomerId = userId,
                       Quantity = dbOrderDetails?.Quantity,
                       Price = dbOrderDetails?.Price,
                       AddressId = 1
                   });
                await _storeContext.SaveChangesAsync();

                if (index == 0) index = _storeContext.OrderDetails.Max(x => x.Id) + 1;

                _storeContext.Orders.Add(new Checkout()
                {
                    SaleId = 1,
                    CartId = _storeContext.OrderDetails.Max(x => x.Id),
                    PaymentId = _storeContext.Payments.Max(x => x.Id),
                    index = index,
                    OrderDate = DateTime.Now,
                });
                await _storeContext.SaveChangesAsync();

                _storeContext.StatusOrders.Add(
                 new StatusOrder()
                 {
                     CreateAt = DateTime.Now,
                     CreateBy = userId,
                     UpdateBy = userId,
                     UpdateAt = DateTime.Now,
                     OrderId = _storeContext.Orders.Max(x => x.Id),
                     Type = "Chờ duyệt"
                 });
                await _storeContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Update), new { id = index });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var userJson = HttpContext.Session.GetString("user");

            // Deserialize JSON string to User object
            var user = JsonConvert.DeserializeObject<User>(userJson ?? "");

            var userId = user?.Id ?? 0;
            var listOrderByIndex = await _storeContext.Orders.Include(x => x.OrderDetails).Where(x => x.index == id).ToListAsync();

            var orderDetails = new List<OrderDetailsModel>();
            foreach (var order in listOrderByIndex)
            {
                var dbOrderDetails = order.OrderDetails;
                var cartProduct = await (from storage in _storeContext.Storages
                                         join product in _storeContext.Products on storage.ProductId equals product.Id
                                         where storage.Id == dbOrderDetails.StorageId
                                         select new
                                         {
                                             productId = product.Id,
                                             imageUrl = product.Image,
                                             productName = product.Name,
                                             price = storage.SalePrice
                                         }).FirstOrDefaultAsync();

                orderDetails.Add(new OrderDetailsModel
                {
                    ProductId = cartProduct?.productId,
                    OrderDetailsId = dbOrderDetails.Id,
                    ProductName = cartProduct?.productName,
                    ImageUrl = cartProduct?.imageUrl,
                    Price = cartProduct?.price,
                    Quantity = dbOrderDetails.Quantity ?? 0,
                    Total = cartProduct?.price * (dbOrderDetails.Quantity ?? 0),
                });

            }

            var firstOrder = listOrderByIndex.FirstOrDefault();
            var statusOrder = await _storeContext.StatusOrders.FirstOrDefaultAsync(x => x.OrderId == firstOrder.Id);

            ViewBag.Order = new OrderModel()
            {
                OrderDate = firstOrder?.OrderDate,
                OrderId = firstOrder?.Id,
                Status = statusOrder?.Type,
                OrderDetails = orderDetails,
                Total = orderDetails.Sum(x => x.Total),
                Index = id,
            };

            ViewBag.Address = await _storeContext.UserAddresses
              .Where(u => u.UserId == userId && u.Status == true)
              .Select(u => u.User_Address)
              .FirstOrDefaultAsync();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string data)
        {
            try
            {
                var updateOrderModels = JsonConvert.DeserializeObject<List<UpdateOrderModel>>(data);

                if (updateOrderModels == null) throw new Exception("No orders");

                foreach (var item in updateOrderModels)
                {
                    var order = await _storeContext.OrderDetails.FirstOrDefaultAsync(x => x.Id == item.OrderId);
                    if (order == null) continue;

                    order.Quantity = item.Quantity;
                    await _storeContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var orders = await _storeContext.Orders.Where(x => x.index == id).ToListAsync();
                foreach (var order in orders)
                {
                    var statusOrder = await _storeContext.StatusOrders.FirstOrDefaultAsync(x => x.OrderId == order.Id);
                    if (statusOrder == null) continue;
                    statusOrder.Type = "Đã hủy";
                    await _storeContext.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
