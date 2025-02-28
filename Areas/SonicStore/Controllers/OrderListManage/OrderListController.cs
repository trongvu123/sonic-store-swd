using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.OrderListManage
{
    [Area("SonicStore")]
    [Authorize(Roles = "saler")]
    public class OrderListController : Controller
    {
        private readonly SonicStoreContext _context;
        public OrderListController(SonicStoreContext context)
        {
            _context = context;
        }

        // GET: OrderListController
        [HttpGet("order-list")]
        public async Task<ActionResult> OrderListScreen()
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

                              group new
                              {
                                  CusName = customer.FullName,
                                  PaymentStatus = paymentStatus.Type,
                                  OrderStatus = orderStatus.Type,
                                  OrderDate = checkout.OrderDate
                              } by new { checkout.index } into groupedData
                              select new
                              {
                                  Index = groupedData.Key.index,
                                  CusName = groupedData.First().CusName,
                                  PaymentStatus = groupedData.First().PaymentStatus,
                                  OrderStatus = groupedData.First().OrderStatus,
                                  OrderDate = groupedData.First().OrderDate
                              }).OrderByDescending(g => g.Index).ToListAsync();

            ViewBag.data = data;

            return View();
        }

    }
}
