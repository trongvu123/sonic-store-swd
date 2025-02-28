using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers
{
    [Area("SonicStore")]
    public class HomeController : Controller
    {
        private readonly SonicStoreContext _context;
        public class ProductInfoViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            public double OriginalPrice { get; set; }
            public double SalePrice { get; set; }

            public string Detail { get; set; } = null!;

            public DateTime? UpdateDate { get; set; }

            public bool Status { get; set; }

            public int CategoryId { get; set; }

            public int BrandId { get; set; }
        }
        public HomeController(SonicStoreContext context)
        {
            _context = context;
        }
        [Route("home")]
        public async Task<IActionResult> HomeScreen()
        {
            int numberOfProducts = 10;
            var product = await _context.Products.Include(p=>p.Storages).Where(p=>p.Status==true).OrderBy(p=> Guid.NewGuid()).Take(numberOfProducts).ToListAsync();
            var productIphone = await _context.Products.Include(p=>p.Storages).Where(p=>p.BrandId==1 && p.Status==true).Take(numberOfProducts).ToListAsync();
            var productSamsung = await _context.Products.Include(p => p.Storages).Where(p => p.BrandId == 2 && p.Status==true).Take(numberOfProducts).ToListAsync();
            var cateBrand = await _context.Brands.Include(b => b.Products).ToArrayAsync();
            ViewBag.cateBrand = cateBrand;
            ViewBag.product = product;
            ViewBag.productSamsung = productSamsung;
            ViewBag.ProductIphone = productIphone;  
            return View();
        }
        [HttpPost("search-key")]
        public async Task<JsonResult> GetProductSearch(string searchTerm)
        {
            var listSearch = await _context.Products.Include(p => p.Storages).Where(p => p.Name.ToLower().Contains(searchTerm.ToLower())).Select(p => new
            {
                p.Id,
                p.Name,
                p.Image,
                salePrice= p.Storages.OrderBy(p => p.SalePrice).Select(p=>p.SalePrice).FirstOrDefault(),
            }).ToListAsync();

            return Json(new { list = listSearch });
        }
    }
}
