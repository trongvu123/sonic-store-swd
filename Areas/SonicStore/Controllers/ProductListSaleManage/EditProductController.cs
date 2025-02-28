using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.ProductListSaleManage
{
    [Area("SonicStore")]
    [Authorize(Roles = "saler")]
    public class EditProductController : Controller
    {

        private readonly SonicStoreContext _context;

        public EditProductController(SonicStoreContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("EditCreen/{id}")]
        public async Task<IActionResult> EditCreen(int id)
        {

            var storage = await _context.Storages.FindAsync(id);
            if (storage != null)
            {
                var product = await _context.Products.FindAsync(storage.ProductId);
                if (product != null)
                {
                    var image = await _context.ProductImages.FindAsync(product.Id);

                    ViewBag.product = product;
                    ViewBag.image = image;
                    ViewBag.storage = storage;
                    return View("EditScreen");
                }
            }

            return Redirect("https://localhost:44390/productsSale");
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit( [FromForm] Inventory storage)
        {
            if (storage != null)
            {
                var existingStorage = await _context.Storages.FindAsync(storage.Id);

                if (existingStorage != null)
                {

                    bool storageChanged = existingStorage.Storage_capacity != storage.Storage_capacity ||
                                          existingStorage.OriginalPrice != storage.OriginalPrice ||
                                          existingStorage.SalePrice != storage.SalePrice ||
                                          existingStorage.quantity != storage.quantity;

                    if (storageChanged)
                    {


                        if (storageChanged)
                        {
                            existingStorage.Storage_capacity = storage.Storage_capacity;
                            existingStorage.OriginalPrice = storage.OriginalPrice;
                            existingStorage.SalePrice = storage.SalePrice;
                            existingStorage.quantity = storage.quantity;
                        }

                        await _context.SaveChangesAsync();
                        string id = storage.Id.ToString();
                        TempData["Message"] = $"Đã thay đổi chi tiết sản phẩm";
                        return RedirectToAction("ProductListSale", "ProductListSale", new { area = "SonicStore" });

                    }

                    TempData["Message"] = $"Không có thông tin nào được thay đổi";
                    return RedirectToAction("ProductListSale", "ProductListSale", new { area = "SonicStore" });

                }


            }
            return RedirectToAction("EditCreen");
        }
    }

}