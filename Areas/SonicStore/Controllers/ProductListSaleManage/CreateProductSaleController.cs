using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Web;

namespace SonicStore.Areas.SonicStore.Controllers.ProductListSaleManage
{
    [Area("SonicStore")]
    [Authorize(Roles = "saler")]

    public class CreateProductSaleController : Controller
    {
        private readonly SonicStoreContext _context;

        public CreateProductSaleController(SonicStoreContext context)
        {
            _context = context;
        }

        [Route("/createps")]
        public async Task<IActionResult> CreateProductSaleScreen()
        {
            var productid = await _context.Products.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
            var brand = await _context.Brands.ToListAsync();
            var category = await _context.Categories.ToListAsync();
            ViewBag.ProductId = productid.Id;
            ViewBag.brand = brand;
            ViewBag.category = category;

            return View();

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromForm] Product product, [FromForm] string filePaths, [FromForm] string storageString)
        {

            try
            {
                // xu ly anh
                List<string> _imgProduct = filePaths.Split("##").ToList();
                List<string> imgProduct = _imgProduct.Skip(1).ToList();
                string img = _imgProduct[0];

                // xu ly storage
                string[] mainParts = storageString.Split(new string[] { ";;" }, StringSplitOptions.None);
                List<string[]> listOfStorage = new List<string[]>();
                foreach (string part in mainParts)
                {
                    string[] subParts = part.Split(new string[] { "##" }, StringSplitOptions.None);
                    listOfStorage.Add(subParts);
                }

                if (product != null)
                {
                    if (_context.Products.FirstOrDefault(p => p.Name.Equals(product.Name)) != null)
                    {
                        TempData["Message"] = "Có lỗi trong quá trình thêm sản phẩm (Tên sản phẩm đã tồn tại)";
                        return Redirect("https://localhost:44390/productsSale");
                    }
                    else
                    {
                        var _produc = new Product
                        {
                            Name = product.Name,
                            Detail = product.Detail,
                            Image = "/images/productimg/" + img,
                            UpdateDate = product.UpdateDate,
                            Status = true,
                            CategoryId = product.CategoryId,
                            BrandId = product.BrandId
                        };
                        _context.Products.Add(_produc);
                        await _context.SaveChangesAsync();
                    }
                }
                int pID = 0;
                foreach (string[] item in listOfStorage)
                {
                    if (int.Parse(item[0]) <= 0 || double.Parse(item[1]) <= 0 || double.Parse(item[2]) <= 0 || int.Parse(item[3]) <= 0)
                    {
                        TempData["Message"] = "Có lỗi trong quá trình thêm sản phẩm (Chi tiết bộ nhớ của điện thoại có vấn đề)";
                        return Redirect("https://localhost:44390/productsSale");
                    }
                    else
                    {

                        var sto = new Inventory
                        {
                            Storage_capacity = int.Parse(item[0]),
                            OriginalPrice = double.Parse(item[1]),
                            SalePrice = double.Parse(item[2]),
                            quantity = int.Parse(item[3]),
                            ProductId = int.Parse(item[4])
                        };
                        _context.Storages.Add(sto);
                        await _context.SaveChangesAsync();
                    }
                }
                foreach (string item in imgProduct)
                {
                    var ip = new ProductImage
                    {
                        ProductId = pID,
                        Image = "/images/productimg/" + item
                    };
                    _context.ProductImages.Add(ip);
                    await _context.SaveChangesAsync(true);
                }

                TempData["Message"] = "Đã thêm sản phẩm thành công";
                return Redirect("https://localhost:44390/productsSale");
            }
            catch
            {
                TempData["Message"] = "Có lỗi trong quá trình thêm sản phẩm";
                return Redirect("https://localhost:44390/productsSale");
            }

        }

    }
}
