using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SonicStore.Areas.SonicStore.Models;
using SonicStore.Areas.SonicStore.Services;
using SonicStore.Areas.SonicStore.Dtos;
namespace SonicStore.Areas.SonicStore.Controllers.CheckoutManage
{
    [Authorize(Roles = "customer")]
    [Area("SonicStore")]
    public class CheckoutController : Controller
    {
        private readonly SonicStoreContext _context;
        public IVnPayService _vnPayService;
        public CheckoutController(SonicStoreContext context, IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
            _context = context;
        }
        [HttpGet("checkout")]
        public async Task<IActionResult> CheckoutScreen()
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var addressUser = await _context.UserAddresses.Where(u => u.UserId == userSession.Id && u.Status == true).FirstOrDefaultAsync();
            List<Cart> listToCheckout = new List<Cart>();
            var listJson = HttpContext.Session.GetString("listCheckout");
            if (listJson == null)
            {
                return View();
            }
            var listSession = JsonConvert.DeserializeObject<List<int>>(listJson);
            decimal totalPrice = 0;
            decimal totalAmountItem = 0;
            foreach (var item in _context.OrderDetails.Include(o => o.Storage).ThenInclude(o => o.Product))
            {
                foreach (var i in listSession)
                {
                    if (item.Id == i)
                    {
                        totalAmountItem = (decimal)item.Quantity * (decimal)item.Storage.SalePrice;
                        totalPrice += (decimal)totalAmountItem;
                        listToCheckout.Add(item);
                    }
                }
            }
            ViewBag.totalPrice = totalPrice;
            ViewBag.userSession = userSession;
            ViewBag.addressUser = addressUser;
            return View(listToCheckout);
        }
        [HttpGet("vip")]
        public IActionResult CheckOutVNPAY()
        {

            var model = new Helper.VnPayRequestModel
            {
                Fullname = "Nguyen manh hung",
                Description = "index",
                Amount = 999999,
                CreatedDate = DateTime.Now,
                OrderId = 1

            };


            return Redirect(_vnPayService.CreatePaymentURL(HttpContext, model));
        }
        [HttpGet("PaymentFailMessage")]
        public IActionResult PaymentFail()
        {
            return View();
        }
        [HttpGet("/Cart/PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            if (Request.QueryString.ToString() == "")
            {
                return RedirectToAction("ErrorScreen", "Error");
            }
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = "Thanh toán không thành công";
                return RedirectToAction("PaymentFail");
            }
            var orderJson = HttpContext.Session.GetString("order");
            userInput orderInfor = null;
            if (orderJson != null)
            {
                orderInfor = JsonConvert.DeserializeObject<userInput>(orderJson);
            }
            if (orderInfor != null)
            {
                var userJson = HttpContext.Session.GetString("user");
                var userSession = JsonConvert.DeserializeObject<User>(userJson);
                var userAddressId = await _context.UserAddresses.Where(u => u.UserId == userSession.Id && u.Status == true).FirstOrDefaultAsync();
                var priceOption = await _context.Storages.Where(s => s.Id == orderInfor.storageId).Select(s => s.SalePrice).FirstOrDefaultAsync();
                var productOption = await _context.Storages.Where(s => s.Id == orderInfor.storageId).FirstOrDefaultAsync();
                productOption.quantity = productOption.quantity - 1;
                _context.Storages.Update(productOption);
                userAddressId.User_Address = $"{orderInfor.xa}, {orderInfor.huyen}, {orderInfor.tinh}";
                _context.UserAddresses.Update(userAddressId);
                var cart = new Cart
                {
                    CustomerId = userSession.Id,
                    AddressId = userAddressId.Id,
                    Quantity = 1,
                    Price = priceOption,
                    StorageId = orderInfor.storageId,
                    Status = "bill"
                };
                _context.OrderDetails.Add(cart);
                await _context.SaveChangesAsync();
                var paymentMethod = new Payment
                {
                    TotalPrice = priceOption,
                    PaymentMethod = "Thanh toán qua VnPay",
                    TransactionDate = DateTime.Now,
                };
                _context.Payments.Add(paymentMethod);
                await _context.SaveChangesAsync();
                var indexCount = await _context.Orders.CountAsync();
                var lastIndex = await _context.Orders.OrderByDescending(o => o.Id).Select(o => o.index).FirstOrDefaultAsync();
                var checkout = new Checkout
                {
                    OrderDate = DateTime.Now,
                    CartId = cart.Id,
                    PaymentId = paymentMethod.Id,
                    index = indexCount > 0 ? lastIndex + 1 : 1
                };
                _context.Orders.Add(checkout);
                await _context.SaveChangesAsync();
                var statusPayment = new StatusPayment
                {
                    Type = "Đã thanh toán",
                    UpdateAt = DateTime.Now,
                    UpdateBy = userSession.Id,
                    CreateAt = DateTime.Now,
                    CreateBy = userSession.Id,
                    Payment_id = paymentMethod.Id,
                };
                _context.StatusPayments.Add(statusPayment);
                await _context.SaveChangesAsync();
                var statusOrder = new StatusOrder
                {
                    Type = "Chờ duyệt",
                    UpdateAt = DateTime.Now,
                    UpdateBy = userSession.Id,
                    CreateAt = DateTime.Now,
                    CreateBy = userSession.Id,
                    OrderId = checkout.Id,
                };
                _context.StatusOrders.Add(statusOrder);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("order");
                await HttpContext.Session.CommitAsync();
                TempData["Message"] = "Thanh toán thành công";

            }
            // case của ô xử lí ở else này
            else
            {
                TempData["Message"] = "Thanh toán thành công";
                await this.changeInDatabase();

            }
            // add database here

            return View();
        }
        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow(string strUser)
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var orderInfor = JsonConvert.DeserializeObject<userInput>(strUser);
            var userAddressId = await _context.UserAddresses.Where(u => u.UserId == userSession.Id && u.Status == true).FirstOrDefaultAsync();
            var priceOption = await _context.Storages.Where(s => s.Id == orderInfor.storageId).Select(s => s.SalePrice).FirstOrDefaultAsync();
            var productOption = await _context.Storages.Where(s => s.Id == orderInfor.storageId).FirstOrDefaultAsync();
            productOption.quantity = productOption.quantity - 1;
            _context.Storages.Update(productOption);
            userAddressId.User_Address = $"{orderInfor.xa}, {orderInfor.huyen}, {orderInfor.tinh}";
            _context.UserAddresses.Update(userAddressId);
            if (orderInfor.ReceiveTypeID == 1)
            {
                var cart = new Cart
                {
                    CustomerId = userSession.Id,
                    AddressId = userAddressId.Id,
                    Quantity = 1,
                    Price = priceOption,
                    StorageId = orderInfor.storageId,
                    Status = "bill"
                };
                _context.OrderDetails.Add(cart);
                await _context.SaveChangesAsync();
                var paymentMethod = new Payment
                {
                    TotalPrice = priceOption,
                    PaymentMethod = "Thanh toán tiền mặt",
                    TransactionDate = DateTime.Now,
                };
                _context.Payments.Add(paymentMethod);
                await _context.SaveChangesAsync();
                var indexCount = await _context.Orders.CountAsync();
                var lastIndex = await _context.Orders.OrderByDescending(o => o.Id).Select(o => o.index).FirstOrDefaultAsync();
                var checkout = new Checkout
                {
                    OrderDate = DateTime.Now,
                    CartId = cart.Id,
                    PaymentId = paymentMethod.Id,
                    index = indexCount > 0 ? lastIndex + 1 : 1
                };
                _context.Orders.Add(checkout);
                await _context.SaveChangesAsync();
                var statusPayment = new StatusPayment
                {
                    Type = "Chưa thanh toán",
                    UpdateAt = DateTime.Now,
                    UpdateBy = userSession.Id,
                    CreateAt = DateTime.Now,
                    CreateBy = userSession.Id,
                    Payment_id = paymentMethod.Id,
                };
                _context.StatusPayments.Add(statusPayment);
                await _context.SaveChangesAsync();
                var statusOrder = new StatusOrder
                {
                    Type = "Chờ duyệt",
                    UpdateAt = DateTime.Now,
                    UpdateBy = userSession.Id,
                    CreateAt = DateTime.Now,
                    CreateBy = userSession.Id,
                    OrderId = checkout.Id,
                };
                _context.StatusOrders.Add(statusOrder);
                await _context.SaveChangesAsync();
                return Json(new { status = true });
            }
            else if (orderInfor.ReceiveTypeID == 2)
            {
                try
                {

                    var orderJson = JsonConvert.SerializeObject(orderInfor);
                    HttpContext.Session.SetString("order", orderJson);
                    var model = new Helper.VnPayRequestModel
                    {
                        Fullname = userSession.FullName,
                        Description = "index",
                        Amount = priceOption,
                        CreatedDate = DateTime.Now,
                        OrderId = new Random().Next(1000, 100000)

                    };
                    var paymentUrl = _vnPayService.CreatePaymentURL(HttpContext, model);

                    return Json(new { url = paymentUrl });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"VNPay URL: {ex.Message}");
                    return StatusCode(500, " payment URL");
                }

            }
            else
            {
                return BadRequest("Invalid ReceiveTypeID");
            }
        }
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOutVNPAY([FromForm] string payment)
        {
            if (payment == "cod")
            {
                await this.changeInDatabaseCOD();
                //return View("CheckoutCODScreen");
                TempData["Message"] = "Đơn hàng của bạn đang chờ duyệt";
                return View("PaymentCallBack");

            }
            else
            {



                var listJson = HttpContext.Session.GetString("listCheckout");
                List<Cart> listToCheckout = new List<Cart>();
                double Amount = 0;
                var listSession = JsonConvert.DeserializeObject<List<int>>(listJson);
                foreach (var item in _context.OrderDetails.Include(o => o.Storage).ThenInclude(o => o.Product))
                {
                    foreach (var i in listSession)
                    {
                        if (item.Id == i)
                        {
                            Amount += (int)item.Quantity * (double)item.Storage.SalePrice;

                        }
                    }
                }
                var _user = HttpContext.Session.GetString("user");
                User user = JsonConvert.DeserializeObject<User>(_user);
                var model = new Helper.VnPayRequestModel
                {
                    Fullname = user.FullName,
                    Description = "index",
                    Amount = Amount,
                    CreatedDate = DateTime.Now,
                    OrderId = 1

                };


                return Redirect(_vnPayService.CreatePaymentURL(HttpContext, model));
            }
        }

        private async Task changeInDatabaseCOD()
        {
            try
            {
                var _user = HttpContext.Session.GetString("user");
                User user = JsonConvert.DeserializeObject<User>(_user);
                List<Cart> listToCheckout = new List<Cart>();
                var listJson = HttpContext.Session.GetString("listCheckout");
                double Amount = 0;
                //chuyen trang thai cart
                var listSession = JsonConvert.DeserializeObject<List<int>>(listJson);
                int ind;
                if (await _context.Orders.CountAsync() == 0)
                {
                    ind = 0;
                }
                else
                {
                    ind = _context.Orders.Max(s => s.index);
                }
                var listCart = await _context.OrderDetails
                    .Where(o => o.CustomerId == user.Id)
                    .Include(o => o.Storage)
                    .ToListAsync();
                foreach (var item in listCart)
                {
                    foreach (var i in listSession)
                    {
                        if (item.Id == i)
                        {
                            Cart od = await _context.OrderDetails.Where(s => s.Id == item.Id).FirstOrDefaultAsync();
                            od.Status = "bill";

                            Amount = (int)item.Quantity * (double)item.Storage.SalePrice;

                            _context.OrderDetails.Update(od);

                            await _context.SaveChangesAsync();

                            Payment p = new Payment
                            {
                                TotalPrice = Amount,
                                PaymentMethod = "COD",
                                TransactionDate = DateTime.Now,
                            };
                            _context.Payments.Add(p);
                            await _context.SaveChangesAsync();
                            Amount = 0;
                            StatusPayment ps = new StatusPayment
                            {
                                Type = "Chưa thanh toán",
                                UpdateAt = DateTime.Now,
                                CreateAt = DateTime.Now,
                                CreateBy = user.Id,
                                UpdateBy = user.Id,
                                Payment_id = p.Id
                            };
                            _context.StatusPayments.Add(ps);
                            await _context.SaveChangesAsync();

                            Checkout o = new Checkout
                            {
                                OrderDate = DateTime.Now,
                                SaleId = user.Id,
                                CartId = item.Id,
                                PaymentId = p.Id,
                                index = ind + 1
                            };
                            _context.Orders.Add(o);
                            await _context.SaveChangesAsync();

                            StatusOrder so = new StatusOrder
                            {
                                Type = "Chờ duyệt",
                                UpdateAt = DateTime.Now,
                                CreateAt = DateTime.Now,
                                CreateBy = user.Id,
                                OrderId = o.Id
                            };
                            _context.StatusOrders.Add(so);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

            }
            catch
            {
                throw;
            }

        }
        private async Task changeInDatabase()
        {
            try
            {
                var _user = HttpContext.Session.GetString("user");
                User user = JsonConvert.DeserializeObject<User>(_user);
                List<Cart> listToCheckout = new List<Cart>();
                var listJson = HttpContext.Session.GetString("listCheckout");
                double Amount = 0;
                //chuyen trang thai cart
                var listSession = JsonConvert.DeserializeObject<List<int>>(listJson);
                int ind;
                if (await _context.Orders.CountAsync() == 0)
                {
                    ind = 0;
                }
                else
                {
                    ind = _context.Orders.Max(s => s.index);
                }
                var listCart = await _context.OrderDetails
                    .Where(o => o.CustomerId == user.Id)
                    .Include(o => o.Storage)
                    .ToListAsync();
                foreach (var item in listCart)
                {
                    foreach (var i in listSession)
                    {
                        if (item.Id == i)
                        {
                            Cart od = await _context.OrderDetails.Where(s => s.Id == item.Id).FirstOrDefaultAsync();
                            od.Status = "bill";

                            Amount = (int)item.Quantity * (double)item.Storage.SalePrice;

                            _context.OrderDetails.Update(od);

                            await _context.SaveChangesAsync();

                            Payment p = new Payment
                            {
                                TotalPrice = Amount,
                                PaymentMethod = "VNPAY",
                                TransactionDate = DateTime.Now,
                            };
                            _context.Payments.Add(p);
                            await _context.SaveChangesAsync();
                            Amount = 0;
                            StatusPayment ps = new StatusPayment
                            {
                                Type = "Đã thanh toán",
                                UpdateAt = DateTime.Now,
                                CreateAt = DateTime.Now,
                                CreateBy = user.Id,
                                UpdateBy = user.Id,
                                Payment_id = p.Id
                            };
                            _context.StatusPayments.Add(ps);
                            await _context.SaveChangesAsync();

                            Checkout o = new Checkout
                            {
                                OrderDate = DateTime.Now,
                                SaleId = user.Id,
                                CartId = item.Id,
                                PaymentId = p.Id,
                                index = ind + 1
                            };
                            _context.Orders.Add(o);
                            await _context.SaveChangesAsync();

                            StatusOrder so = new StatusOrder
                            {
                                Type = "Chờ duyệt",
                                UpdateAt = DateTime.Now,
                                CreateAt = DateTime.Now,
                                CreateBy = user.Id,
                                OrderId = o.Id
                            };
                            _context.StatusOrders.Add(so);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

            }
            catch
            {
                throw;
            }

        }

    }
}
