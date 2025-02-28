using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.ProductListSaleManage
{
    public class RemoveSaleprice : Controller
    {
        private readonly SonicStoreContext _context;

        public RemoveSaleprice(SonicStoreContext context)
        {
            _context = context;
        }

        [HttpPost("RemoveSaleprice/DeleteSale/{id:int?}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if (storage != null)
            {
                var s = await _context.Storages.Where(s => s.Id == storage.Id).Include(s => s.Product).Select(p => p.Product.Name).FirstOrDefaultAsync();
                if (storage.SalePrice != storage.OriginalPrice)
                {
                    storage.SalePrice = storage.OriginalPrice;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Sản phẩm {s}, giá bán sẽ bằng giá gốc";
                }
                else
                {
                    TempData["Message"] = $"Sản phẩm {s} giá bán đã bằng giá gốc, không thể thay đôi";
                }
            }

            return Redirect("https://localhost:44390/productsSale");
        }


    }
}
