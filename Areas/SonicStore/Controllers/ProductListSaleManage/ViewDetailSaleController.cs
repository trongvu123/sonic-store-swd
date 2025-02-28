using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;

namespace SonicStore.Areas.SonicStore.Controllers.ProductListSaleManage
{
    [Area("SonicStore")]
    [Authorize(Roles = "saler")]
    public class ViewDetailSaleController : Controller
    {
        private readonly SonicStoreContext _context;

        public ViewDetailSaleController(SonicStoreContext context)
        {
            _context = context;
        }
        // GET: ViewDetailSaleController
        [HttpPost("/detail/{id:int?}")]
        public async Task<IActionResult> ViewDetailSale(int id)
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
                    return View("ViewDetailSaleScreen");
                }
            }

            return RedirectToAction("ProductListSale", "ProductListSale", new { area = "SonicStore" });

        }


    }
}