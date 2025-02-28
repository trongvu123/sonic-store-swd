using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.ProductDetailManage
{
    [Authorize(Roles ="customer")]
    [Area("SonicStore")]
    public class ProductDetailController : Controller
    {
        private readonly SonicStoreContext _context;

        public ProductDetailController(SonicStoreContext context)
        {
            _context = context;
        }
        [HttpGet("product-detail/{id?}")]
        public async Task<IActionResult> ProductDetailScreen(int? id, int? storageId)
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var productFeedback = await _context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            var brandID = await _context.Products.Where(p => p.Id == id).Select(p => p.BrandId).FirstOrDefaultAsync();
              var  listProductRelate = await _context.Brands
                                    .SelectMany(b => b.Products).Include(p=>p.Storages)
                                    .Where(b=>b.BrandId==brandID && b.Id!=id)
                                    .Take(3)
                                    .ToListAsync();
           var product = await _context.Products.Where(p=>p.Id==id).Include(s=>s.Storages).Include(i=>i.ProductImages).FirstOrDefaultAsync();
           string address = await _context.UserAddresses.Where(u=>u.UserId == userSession.Id && u.Status==true).Select(u=>u.User_Address).FirstOrDefaultAsync();
           string [] arr = address.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            ViewBag.AddressParts = arr;
            ViewBag.listProductRelate = listProductRelate;
           ViewBag.product=product;
            ViewBag.userSession=userSession;
            ViewBag.productFeedback=productFeedback;

           return View(listProductRelate);
        }
        [HttpPost("add-item")]
        public async Task<JsonResult> AddProductToCart(int id)
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var productItem = await _context.OrderDetails.FirstOrDefaultAsync(od=>od.StorageId==id && od.Status=="cart" && od.CustomerId==userSession.Id);
            var productOption = await _context.Storages.Where(s=>s.Id==id).FirstOrDefaultAsync();
            var userAddressId = await _context.UserAddresses.Where(u => u.UserId == userSession.Id && u.Status == true).FirstOrDefaultAsync();
            var product = await _context.Storages.FindAsync(id);
            int check = 1;
            if (productItem == null)
            {
                var cart = new Cart
                {
                    StorageId = id,
                    CustomerId = userSession.Id,
                    Quantity = 1,
                    Price = product.SalePrice,
                    AddressId = userAddressId.Id,
                    Status = "cart"
                };
                await _context.OrderDetails.AddAsync(cart);
                await _context.SaveChangesAsync();
            }
            else
            {
                productItem.Quantity += 1;
                productItem.Price += product.SalePrice;
                _context.OrderDetails.Update(productItem);
                if (productItem.Quantity > productOption.quantity-1)
                {
                    check = 2;
                }
                else if(productOption.quantity == 0)
                {
                    check = 3;
                }
                else
                {
                  await _context.SaveChangesAsync();

                }
            }
            
            return Json(new { status = check });
        }
    }
}
