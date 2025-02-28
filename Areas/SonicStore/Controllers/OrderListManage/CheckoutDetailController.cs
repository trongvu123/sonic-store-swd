using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.OrderListManage
{
    [Area("SonicStore")]
    [Authorize(Roles = "saler")]

    public class CheckoutDetailController : Controller
    {
        public CheckoutDetailController()
        {
            _context = new SonicStoreContext();
        }
        private readonly SonicStoreContext _context;

        [HttpPost("/checkout-detail/{id:int?}")]
        public async Task<IActionResult> CheckoutDetail(int id)

        {
            var data = await (from checkout in _context.Orders
                              join cart in _context.OrderDetails
             on checkout.CartId equals cart.Id

                              join payment in _context.Payments
                              on checkout.PaymentId equals payment.Id

                              join orderStatus in _context.StatusOrders
                              on checkout.Id equals orderStatus.OrderId

                              join paymentStatus in _context.StatusPayments
                              on payment.Id equals paymentStatus.Payment_id

                              join customer in _context.Users
                              on cart.CustomerId equals customer.Id

                              join storage in _context.Storages
                              on cart.StorageId equals storage.Id

                              join product in _context.Products
                              on storage.ProductId equals product.Id
                              where checkout.index == id

                              select new
                              {
                                  proId = product.Id,
                                  proName = product.Name,
                                  storage = storage.Storage_capacity,
                                  cusName = customer.FullName,
                                  index = checkout.index,
                                  paymentStatus = paymentStatus.Type,
                                  OrderStatus = orderStatus.Type,
                                  orderDate = checkout.OrderDate,
                                  stock = storage.quantity,
                                  num = cart.Quantity,
                              }
                  ).ToListAsync();
            ViewBag.data = data;
            return View("CheckoutDetail");
        }

        [HttpPost("/checkout-done/{id:int?}")]

        public async Task<IActionResult> CheckoutDone(int id)
        {
            var data = await (from checkout in _context.Orders
                              join cart in _context.OrderDetails
             on checkout.CartId equals cart.Id

                              join payment in _context.Payments
                              on checkout.PaymentId equals payment.Id

                              join orderStatus in _context.StatusOrders
                              on checkout.Id equals orderStatus.OrderId

                              join paymentStatus in _context.StatusPayments
                              on payment.Id equals paymentStatus.Payment_id

                              where checkout.index == id

                              select new
                              {
                                  paymentStatusId = paymentStatus.Id,
                                  orderStatusId = orderStatus.Id,
                                  index = checkout.index,
                                  paymentStatus = paymentStatus.Type,
                                  orderStatus = orderStatus.Type
                              }
                 ).ToListAsync();
            foreach (var item in data)
            {
                try
                {
                    var statusOrder = await _context.StatusOrders.Where(o => o.Id == item.orderStatusId).FirstOrDefaultAsync();

                    statusOrder.Type = "Vận chuyển thành công";
                    _context.StatusOrders.Update(statusOrder);
                    var paySatus = await _context.StatusPayments.Where(o => o.Id == item.paymentStatusId).FirstOrDefaultAsync();
                    paySatus.Type = "Đã thanh toán";
                    _context.StatusPayments.Update(paySatus);
                    await _context.SaveChangesAsync();


                }
                catch
                {
                    TempData["Message"] = $"Duyệt không thành công đơn hàng";
                    return RedirectToAction("OrderListScreen", "OrderList", new { area = "SonicStore" });

                }


            }
            TempData["Message"] = $"Duyệt thành công đơn hàng";

            return RedirectToAction("OrderListScreen", "OrderList", new { area = "SonicStore" });

        }

        [HttpPost("/checkout-comfirm/{id:int?}")]
        public async Task<IActionResult> ComfirmCheckout(int id)

        {

            var data = await (from checkout in _context.Orders
                              join cart in _context.OrderDetails
             on checkout.CartId equals cart.Id

                              join payment in _context.Payments
                              on checkout.PaymentId equals payment.Id

                              join orderStatus in _context.StatusOrders
                              on checkout.Id equals orderStatus.OrderId

                              join paymentStatus in _context.StatusPayments
                              on payment.Id equals paymentStatus.Payment_id


                              join storage in _context.Storages
                              on cart.StorageId equals storage.Id

                              where checkout.index == id

                              select new
                              {
                                  orderStatusId = orderStatus.Id,
                                  storageId = storage.Id,
                                  index = checkout.index,
                                  paymentStatus = paymentStatus.Type,
                                  orderStatus = orderStatus.Type,
                                  orderDate = checkout.OrderDate,
                                  stock = storage.quantity,
                                  num = cart.Quantity,
                              }
                  ).ToListAsync();
            foreach (var item in data)
            {
                try
                {
                    var statusOrder = await _context.StatusOrders.Where(o => o.Id == item.orderStatusId).FirstOrDefaultAsync();

                    statusOrder.Type = "Đang vận chuyển";
                    _context.StatusOrders.Update(statusOrder);
                    await _context.SaveChangesAsync();

                    var storage = await _context.Storages.Where(o => o.Id == item.storageId).FirstOrDefaultAsync();
                    storage.quantity = (int)storage.quantity - (int)item.num;
                    _context.Storages.Update(storage);
                    await _context.SaveChangesAsync();

                }
                catch
                {
                    TempData["Message"] = $"Duyệt không thành công đơn hàng";
                    return RedirectToAction("OrderListScreen", "OrderList", new { area = "SonicStore" });

                }


            }
            TempData["Message"] = $"Duyệt thành công đơn hàng";

            return RedirectToAction("OrderListScreen", "OrderList", new { area = "SonicStore" });
        }
    }
}
