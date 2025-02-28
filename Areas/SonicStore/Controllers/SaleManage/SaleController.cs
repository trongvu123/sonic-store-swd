using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.SaleManage
{
    [Area("SonicStore")]
    public class SaleController : Controller
    {
        private readonly SonicStoreContext _context;

        public SaleController()
        {
            _context = new SonicStoreContext();
        }

        [HttpGet("sale-json")]
        public async Task<IActionResult> SaleScreen()
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
                              where orderStatus.Type.Equals("Vận chuyển thành công")
                              group new
                              {

                                  Email = customer.Email,
                                  CusName = customer.FullName,
                                  PaymentStatus = paymentStatus.Type,
                                  OrderStatus = orderStatus.Type,
                                  OrderDate = checkout.OrderDate
                              } by new { checkout.index } into groupedData
                              select new
                              {
                                  Email = groupedData.First().Email,
                                  Index = groupedData.Key.index,
                                  CusName = groupedData.First().CusName,
                                  PaymentStatus = groupedData.First().PaymentStatus,
                                  OrderStatus = groupedData.First().OrderStatus,
                                  OrderDate = groupedData.First().OrderDate
                              }).ToListAsync();
            ViewBag.data = data;

            var amount = await (from checkout in _context.Orders
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
                                where orderStatus.Type.Equals("Vận chuyển thành công")
                                select new
                                {
                                    Index = checkout.index,
                                    Amount = payment.TotalPrice,
                                    productid = checkout.PaymentId,
                                    numpro = checkout.Id
                                }
                                ).ToListAsync();

            var data2 = await (from checkout in _context.Orders
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
                               where orderStatus.Type.Equals("Vận chuyển thành công")
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
                               }).ToListAsync();
            int sodon = data2.Count(o => o.Index != 0);
            decimal doanhthu = (decimal)amount.Sum(o => o.Amount);
            int numdongiao, numdonhuy;
            var dataorder = await (from checkout in _context.Orders
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
                                   }).ToListAsync();
            numdongiao = dataorder.Where(o => o.OrderStatus.Equals("Đang vận chuyển")).Count();
            numdonhuy = dataorder.Where(o => o.OrderStatus.Equals("Đã hủy")).Count();

            int[] data_giao = new int[12];
            int[] data_huy = new int[12];

            for (int i = 0; i < 12; i++)
            {
                data_giao[i] = dataorder.Where(o => o.OrderStatus.Equals("Vận chuyển thành công") && o.OrderDate.Value.Month == (i + 1)).Count();

                data_huy[i] = dataorder.Where(o => o.OrderStatus.Equals("Đã hủy") && o.OrderDate.Value.Month == (i + 1)).Count();
            }

            var detail = new
            {
                numdonhuy = numdonhuy,
                numdongiao = numdongiao,

                numdon = sodon,
                totalp = doanhthu

            };
            ViewBag.detail = detail;
            ViewBag.DataGiao = data_giao;
            ViewBag.DataHuy = data_huy;

            return View();
        }

    }
}
