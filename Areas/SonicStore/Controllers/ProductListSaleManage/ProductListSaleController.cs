using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SonicStore.Areas.SonicStore.Controllers
{
    [Area("SonicStore")]
    [Authorize(Roles = "saler")]
    public class ProductListSaleController : Controller
    {
        private readonly SonicStoreContext _context;

        public ProductListSaleController(SonicStoreContext context)
        {
            _context = context;
        }

        [HttpGet("/productsSale")]
        public async Task<IActionResult> ProductListSale()
        {
            var listProduct = await (from p in _context.Products
                                     join s in _context.Storages on p.Id equals s.ProductId
                                     select new
                                     {
                                         StorageID = s.Id,
                                         Id = p.Id,
                                         Name = p.Name,
                                         Storage = s.Storage_capacity,
                                         OldPrice = s.OriginalPrice,
                                         SalePrice = s.SalePrice,
                                         Quantity = s.quantity,
                                         Status = p.Status
                                     }).ToListAsync();

            ViewBag.ProductList = listProduct;
            return View();
        }

        [HttpPost("/UpdateProductStatus/{id:int?}")]
        public async Task<IActionResult> UpdateProductStatus(int? id)
        {
            if (id == null)
            {
                return BadRequest("Product ID is required.");
            }

            var product = await _context.Products.FindAsync(id.Value);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            product.Status = !product.Status;
            await _context.SaveChangesAsync();

            TempData["Message"] = product.Status ? $"Sản phẩm {product.Name} được hiển thị." : $"Sản phẩm {product.Name} bị ẩn.";
            return RedirectToAction(nameof(ProductListSale));
        }

        private async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}
