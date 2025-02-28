using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Configuration;
using SonicStore.Areas.SonicStore.Models;
using SonicStore.Areas.SonicStore.Dtos;

namespace SonicStore.Areas.SonicStore.Controllers.CartManage
{
    [Authorize(Roles = "customer")]
    [Area("SonicStore")]
    public class CartController : Controller
    {
        private readonly SonicStoreContext _context;

        public CartController(SonicStoreContext context)
        {
            _context = context;
        }

        [HttpGet("cart")]
        public async Task<IActionResult> CartScreen()
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var user = _context.Users.FirstOrDefault();
            var addressUser = await _context.UserAddresses.Where(u => u.UserId == userSession.Id && u.Status == true).Select(a => a.User_Address).FirstOrDefaultAsync();
            ViewBag.AddressUser = addressUser;
            ViewBag.user = user;
            return View();
        }
        [HttpGet("loaddataCart")]
        public async Task<JsonResult> loadData()
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
			var listJson = HttpContext.Session.GetString("listCheckout");

			List<int> listSession = new List<int>();
			if (!string.IsNullOrEmpty(listJson))
			{
				listSession = JsonConvert.DeserializeObject<List<int>>(listJson) ?? new List<int>();
			}
			var listCartItem = await _context.OrderDetails.Where(od => od.CustomerId == userSession.Id && od.Status=="cart" && od.Storage.Product.Status == true).Include(u => u.User)
	                            .Include(u => u.UserAddress)
	                            .Include(s => s.Storage)
	                            .ThenInclude(p => p.Product)
                                .Select(c=> new
                                {
                                    c.Id,
                                    c.Price,
                                    c.StorageId,
                                    c.Storage.Product.Name,
									c.Storage.Product.Image,
									c.Storage.SalePrice,
									StorageQuantity = c.Storage.quantity,
                                    c.Quantity,
									isCheck = listSession.Contains(c.Id)
								})
	                            .ToListAsync();
            return Json(new { data = listCartItem , status = true});
		}
        [HttpPost("update-quantity")]
        public async Task<JsonResult> UpdateQuantity(string? quantity, int? id, int? all)
        {
            int status = 0;
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var allItem = await _context.OrderDetails.Where(o => o.CustomerId == userSession.Id && o.Status == "cart").ToListAsync();
            var productOption = await _context.OrderDetails.Include(o => o.Storage).Where(o => o.Id == id).Select(o => o.Storage).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(quantity))
            {
                var cartItem = await _context.OrderDetails.Where(c => c.Id == id).FirstOrDefaultAsync();
                var unitPrice = await _context.OrderDetails.Where(c => cartItem.StorageId == c.Storage.Id).Include(c => c.Storage).Select(s => s.Storage.SalePrice).FirstOrDefaultAsync();

                if (int.TryParse(quantity, out var quantityInput))
                {
                    cartItem.Quantity = quantityInput;
                    cartItem.Price = quantityInput * unitPrice;
                    status = 1;
                    _context.OrderDetails.Update(cartItem);
                }
                else
                {
                    if (quantity == "down")
                    {
                        if (cartItem.Quantity <= 1)
                        {
                            status = 2;
                            cartItem.Quantity = 1;
                            _context.OrderDetails.Update(cartItem);
                        }                     
                        else
                        {
                            cartItem.Quantity -= 1;
                            cartItem.Price -= unitPrice;
                            status = 1;
                            _context.OrderDetails.Update(cartItem);
                        }
                    }
                    else
                    {
                        if (cartItem.Quantity > productOption.quantity - 1)
                        {
                            status = 3;
                        }
                        else
                        {
                            cartItem.Quantity += 1;
                            cartItem.Price += unitPrice;
                            status = 1;
                            _context.OrderDetails.Update(cartItem);
                        }
                    }
                }
            }
            if (all.HasValue)
            {
                _context.OrderDetails.RemoveRange(allItem);

            }
            await _context.SaveChangesAsync();
            return Json(new { status = status });
        }
        [HttpPost("delete-item")]
        public async Task<JsonResult> DeleteItem(int id)
        {
            var cartItem = await _context.OrderDetails.Where(c => c.Id == id).FirstOrDefaultAsync();
            _context.OrderDetails.Remove(cartItem);
            try
            {
                await _context.SaveChangesAsync();
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = ex.Message });
            }
        }
        [HttpGet("change-adress")]
        public async Task<IActionResult> CartAdressScreen()
        {
            return View();
        }
        [HttpGet("loadDataAddress")]
        public async Task<JsonResult> loadAddress()
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var addressDefault = await _context.UserAddresses.Where(u => u.User.Id == userSession.Id && u.Status == true).Select(u => u.User_Address).FirstOrDefaultAsync();
            var listAddressUser = await _context.UserAddresses.Where(u => u.User.Id == userSession.Id).Include(u => u.User).Select(u => new
            {
                u.Id,
                u.User.FullName,
                u.User_Address,
                u.User.Phone,
                u.Status

            }).ToListAsync();
            return Json(new
            {
                data = listAddressUser,
                status = true
            });
        }
        [HttpPost("update-address")]
        public async Task<JsonResult> updateAdress(int edit)
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var userAddress = await _context.UserAddresses.FindAsync(edit);
            bool flag = userAddress.Status;
            string address = await _context.UserAddresses.Where(u => u.UserId == userSession.Id && u.Status == true).Select(u => u.User_Address).FirstOrDefaultAsync();
            string[] arr = address.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            ViewBag.AddressParts = arr;
            return Json(new { address = arr, status = true, flag = flag });
        }
        [HttpPost("save-address")]
        public async Task<JsonResult> saveAddress(string strAddress)
        {
            bool status = true;
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var address = JsonConvert.DeserializeObject<AddressInput>(strAddress);
            string fullAddress = $"{address.xa}, {address.huyen}, {address.tinh}";
            var userAddress = await _context.UserAddresses.FindAsync(address.id);
            var addressExist = await _context.UserAddresses.Where(u => u.Id != address.id && u.User_Address.Equals(fullAddress) && u.UserId == userSession.Id).FirstOrDefaultAsync();
            if (userAddress.Status == true)
            {
                if (address.check == false)
                {
                    status = true;
                }
                else
                {
                    if (addressExist != null)
                    {
                        status = false;
                    }
                    else
                    {
                        if (address.check == true)
                        {
                            foreach (var item in _context.UserAddresses)
                            {
                                if (item.Id != address.id && item.Status == true)
                                {
                                    item.Status = false;
                                    _context.Update(item);
                                    break;
                                }
                            }
                            userAddress.User_Address = fullAddress;
                            userAddress.Status = true;
                            _context.Update(userAddress);

                        }
                        else
                        {
                            userAddress.User_Address = fullAddress;
                            _context.Update(userAddress);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return Json(new { status = status });
        }
        [HttpPost("change-default")]
        public async Task<JsonResult> ChangeDefaultAdress(int id)
        {
            var userId = 1;
            var address = await _context.UserAddresses.FindAsync(id);
            address.Status = true;
            foreach (var item in _context.UserAddresses)
            {
                if (item.Id != id)
                {
                    item.Status = false;
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { status = true });
        }
        [HttpPost("delete-address")]
        public async Task<JsonResult> DeleteAddress(int delete)
        {
            var addressUser = await _context.UserAddresses.FindAsync(delete);
            if (addressUser != null)
            {
                _context.UserAddresses.Remove(addressUser);
            }
            await _context.SaveChangesAsync();
            return Json(new { status = true });
        }
        [HttpPost("add-address")]
        public async Task<JsonResult> AddNewAddress(string strAddress)
        {
            bool status = true;
            try
            {
                var userJson = HttpContext.Session.GetString("user");
                var userSession = JsonConvert.DeserializeObject<User>(userJson);
                var address = JsonConvert.DeserializeObject<AddressInputAdding>(strAddress);
                string fullAddress = $"{address.xa}, {address.huyen}, {address.tinh}";
                var addressExist = await _context.UserAddresses.Where(u => u.User_Address.Equals(fullAddress) && u.UserId == userSession.Id).FirstOrDefaultAsync();
                if (addressExist == null)
                {
                    if (address.check == true)
                    {
                        foreach (var item in _context.UserAddresses)
                        {
                            if (item.Status == true)
                            {
                                item.Status = false;
                                _context.Update(item);
                                break;
                            }
                        }
                        var newAddress = new UserAddress
                        {
                            User_Address = fullAddress,
                            UserId = userSession.Id,
                            Status = true
                        };
                        _context.UserAddresses.Add(newAddress);
                    }
                    else
                    {
                        var newAddress = new UserAddress
                        {
                            User_Address = fullAddress,
                            UserId = userSession.Id,
                            Status = false
                        };
                        _context.UserAddresses.Add(newAddress);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    status = false;
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return Json(new { status = status });
        }
        [HttpPost("buy-product")]
        public async Task<IActionResult> BuyProduct([FromBody] List<int> listProduct)
        {

            if (listProduct == null || listProduct.Count == 0)
            {
                return BadRequest("No products selected.");
            }

            var listProductJson = JsonConvert.SerializeObject(listProduct);
            HttpContext.Session.SetString("listCheckout", listProductJson);

            return Ok(new { redirectUrl = Url.Action("CheckoutScreen", "Checkout") });
        }
    }
}
