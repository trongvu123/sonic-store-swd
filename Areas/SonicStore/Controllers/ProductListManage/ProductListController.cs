using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.ProductList
{
	[Area("SonicStore")]
    [AllowAnonymous()]
	public class ProductListController : Controller
	{
		private readonly SonicStoreContext _context;
		private const int loadPerItem = 10;
		public ProductListController(SonicStoreContext context)
		{
			_context = context;
		}
		[HttpGet("product-list")]
        public async Task<IActionResult> ProductListScreen()
        {
            var listBrand = await _context.Brands.Include(p => p.Products).ToListAsync();
            var listStorage = await _context.Storages.Select(s => s.Storage_capacity).Distinct().ToListAsync();
            ViewBag.listBrand = listBrand;
            ViewBag.listStorage = listStorage;
            return View();
        }
        [HttpGet("loaddata")]
		public async Task<JsonResult> LoadData(int? brand,string? price,int? ram,int? category,int? storage, int? sort, int? delete,int page, int pageSize=10 , int skip=0)
		{
            IQueryable<Product> query = _context.Products.Include(s => s.Storages);
            var priceLow = 0;
            var priceHigh = 0;
            if (delete.HasValue)
            {
                // Reset all filters
                brand = null;
                price = null;
                ram = null;
                category = null;
                storage = null;
                sort = null;
                query = _context.Products.Include(s => s.Storages);
            }
            else
            {

                if (brand != null)
                {
                    query = query.Where(b=>b.BrandId == brand);
                }

                if (!string.IsNullOrWhiteSpace(price))
                {
                    var range = price.Split(" đến ");
                    if (range.Length == 2 && int.TryParse(range[0], out  priceLow) && int.TryParse(range[1], out  priceHigh))
                    {
                        priceLow *= 1000000;
                        priceHigh *= 1000000;
						query = query.Where(p => p.Storages.Any(s => s.SalePrice >= priceLow && s.SalePrice <= priceHigh));
					}
                }
                if (category.HasValue)
                {
                    query = query.Where(p => p.CategoryId==category);
                }
                if (sort.HasValue && !string.IsNullOrEmpty(price))
                {
                    if (sort == 0)
                    {
                        query = query.OrderBy(p=>p.Storages.Where(s => s.SalePrice >= priceLow && s.SalePrice <= priceHigh).Min(s=>s.SalePrice));
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.Storages.Where(s => s.SalePrice >= priceLow && s.SalePrice <= priceHigh).Max(s => s.SalePrice));
                    }
                }
                if(sort.HasValue && string.IsNullOrEmpty(price))
                {
                    if (sort == 0)
                    {
                        query = query.OrderBy(p => p.Storages.Min(s => s.SalePrice));
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.Storages.Max(s => s.SalePrice));
                    }
                }
                if(storage.HasValue)
                {
                    query = query.Where(p=>p.Storages.Any(s=>s.Storage_capacity == storage));
                }
            }

            var listProduct = await query.Skip((page-1)*pageSize).Take(pageSize).
              Select(p => new
              {
                  p.Id,
                  p.Name,
                  p.Image,
                  OriginalPrices = (!string.IsNullOrEmpty(price)) ? p.Storages.Where(s => s.SalePrice >= priceLow && s.SalePrice <= priceHigh).
                  Select(s => s.OriginalPrice).FirstOrDefault() : p.Storages.Select(s => s.OriginalPrice).FirstOrDefault(),
                  SalePrices = (!string.IsNullOrEmpty(price)) ? p.Storages.Where(s => s.SalePrice >= priceLow && s.SalePrice <= priceHigh)
                  .Select(s => s.SalePrice).FirstOrDefault() : p.Storages.Select(s => s.SalePrice).FirstOrDefault(),
                  Url = Url.Action("ProductDetailScreen", "ProductDetail", new { id = p.Id })
              })
                 .ToListAsync();

            var totalRow = 0;
            if (brand != null || price != null || ram != null || category != null || storage != null)
            {
                totalRow = await query.CountAsync();
            }
            else if(delete != null)
            {
                totalRow = await _context.Products.CountAsync();
            }
            else
            {
                totalRow = await _context.Products.CountAsync();
            }
            return Json(new
            {
                Data = listProduct,
                total = totalRow,
                status = true
            });
        }
	}
}
