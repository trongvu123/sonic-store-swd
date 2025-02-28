using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.ProductListManage
{
    [Area("SonicStore")]
    public class PhoneModelController : Controller
    {
        private readonly SonicStoreContext _context;

        public PhoneModelController(SonicStoreContext context)
        {
            _context = context;
        }
        [HttpGet("phone-model")]
        public async Task<IActionResult> PhoneModelScreen(string name, string? fname, int? brand, string? brandName)
        {
            IQueryable<Product> listProduct = _context.Products.Include(s => s.Storages);
            if (string.IsNullOrEmpty(name) && !brand.HasValue)
            {
                return RedirectToAction("HomeScreen", "Home");
            }
            if (!string.IsNullOrEmpty(name))
            {
                ViewBag.fname = fname;
                listProduct = listProduct.Where(p => p.Name.ToLower().Contains(name.ToLower()) && p.Status == true);
                ViewBag.name = name;
            }

            if (brand.HasValue)
            {
                listProduct = listProduct.Where(p => p.BrandId == brand && p.Status==true);
                var listBrand = await _context.Brands.ToListAsync();
                ViewBag.listBrand = listBrand;
                ViewBag.brandName = brandName;
                ViewBag.brand = brand;
            }
            return View(listProduct);
        }
    }
}
