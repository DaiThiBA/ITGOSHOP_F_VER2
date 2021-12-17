﻿using ITGoShop_F_Ver2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ITGoShop_F_Ver2.Controllers
{
    public class ProductManagementController : Controller
    {
        [Obsolete]
        private IHostingEnvironment Environment;

        [Obsolete]
        public ProductManagementController(IHostingEnvironment _environment)
        {
            Environment = _environment;
        }
        public IActionResult add_product()
        {
            ITGoShopContext context = HttpContext.RequestServices.GetService(typeof(ITGoShop_F_Ver2.Models.ITGoShopContext)) as ITGoShopContext;
            ViewBag.AllCategory = context.getAllCategory();
            ViewBag.AllBrand = context.getAllBrand();
            ViewBag.AllSubBrand = context.getAllSubBrand();
            ViewBag.AllBannerSlider = context.getAllBannerSlider();
            return View();
        }

        [Obsolete]
        public IActionResult SaveProduct(Product newProduct, List<IFormFile> ProductImage)
        {
            // Lưu ảnh sản phẩm vào trước
            string path = Path.Combine(this.Environment.WebRootPath, "public/images_upload/product");
            foreach (IFormFile postedFile in ProductImage)
            {
                // Lấy tên file
                newProduct.ProductImage = DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + postedFile.FileName;
                // Lưu file vào project
                string fileName = Path.GetFileName(DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
            }

            var context = new ITGoShopLINQContext();
            context.saveProduct(newProduct);
            return RedirectToAction("add_product");

        }

        public IActionResult view_product()
        {
            ITGoShopContext context = HttpContext.RequestServices.GetService(typeof(ITGoShop_F_Ver2.Models.ITGoShopContext)) as ITGoShopContext;
            ViewBag.AllProduct = context.getAllProduct();
            return View();
        }
        public void unactive_product(int ProductId)
        {
            ITGoShopContext context = HttpContext.RequestServices.GetService(typeof(ITGoShop_F_Ver2.Models.ITGoShopContext)) as ITGoShopContext;
            context.updateProductStatus(ProductId, 0); 
        }
        public void active_product(int ProductId)
        {
            ITGoShopContext context = HttpContext.RequestServices.GetService(typeof(ITGoShop_F_Ver2.Models.ITGoShopContext)) as ITGoShopContext;
            context.updateProductStatus(ProductId, 1);
        }

        public void delete_product(int ProductId)
        {
            ITGoShopContext context = HttpContext.RequestServices.GetService(typeof(ITGoShop_F_Ver2.Models.ITGoShopContext)) as ITGoShopContext;
            context.deleteProduct(ProductId);
        }

        public IActionResult update_product(int productId)
        {
            ITGoShopContext context = HttpContext.RequestServices.GetService(typeof(ITGoShop_F_Ver2.Models.ITGoShopContext)) as ITGoShopContext;
            ViewBag.ProductInfo = context.getProductInfo(productId);
            ViewBag.AllCategory = context.getAllCategory();
            ViewBag.AllBrand = context.getAllBrand();
            ViewBag.AllSubBrand = context.getAllSubBrand();
            ViewBag.AllBannerSlider = context.getAllBannerSlider();
            return View();
        }

        [Obsolete]
        public IActionResult save_update_product(Product product, List<IFormFile> productImage)
        {
            //System.Diagnostics.Debug.WriteLine("PImg: " + product.ProductImage +"-"+ product.ProductName);
            if (!string.IsNullOrEmpty(product.ProductImage))
            {
                //Lưu ảnh sản phẩm vào trước
                string path = Path.Combine(this.Environment.WebRootPath, "public/images_upload/product");
                foreach (IFormFile postedFile in productImage)
                {
                    // Lấy tên file
                    product.ProductImage = DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + postedFile.FileName;
                    // Lưu file vào project
                    string fileName = Path.GetFileName(DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + postedFile.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }
                }
            }
            var context = new ITGoShopLINQContext();
            context.updateProduct(product);
            return RedirectToAction("view_product");

        }
        public IActionResult view_productgallary(int productId)
        {
            var context = new ITGoShopLINQContext();
            ViewBag.ProductGallary = context.getProductGallary(productId);
            ViewBag.ProductId = productId;
            return View();
        }

        public void update_productgallery_status(int GallaryId, int Status)
        {
            var context = new ITGoShopLINQContext();
            context.updateProductGallaryStatus(GallaryId, Status);
        }

        public void delete_productgallery(int GallaryId)
        {
            var context = new ITGoShopLINQContext();
            context.deleteProductGallary(GallaryId);
        }

        [Obsolete]
        public IActionResult add_productgallery(int productId, List<IFormFile> ProductGallary)
        {
            var context = new ITGoShopLINQContext();

            string path = Path.Combine(this.Environment.WebRootPath, "public/images_upload/product-gallary");
            foreach (IFormFile postedFile in ProductGallary)
            {
                ProductGallary productGallary = new ProductGallary()
                {
                    GallaryImage = DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + postedFile.FileName,
                    GallaryStatus = 1,
                    UserId = (int)HttpContext.Session.GetInt32("adminId"),
                    ProductId = productId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                context.saveProductGallary(productGallary);
                // Lưu file vào project
                string fileName = Path.GetFileName(DateTime.Now.ToString("yyyy_MM_dd_HHmmss_") + postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
            }
            return RedirectToAction("view_productgallary", new { productId = productId });
        }

        public ActionResult ExportProduct()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var data = new DataTable();
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add($"id");
                sheet.Cells["A1:H99"].Style.Font.Name = "Times New Roman";
                sheet.Cells["A1:C1"].Merge = true;
                sheet.Column(3).Width = 25;
                sheet.Column(1).Width = 5.33;
                sheet.Column(2).Width = 11.67;
                sheet.Column(5).Width = 20;
                //sheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["A1:H1"].Value = "CÔNG TY CỔ PHẦN THƯƠNG MẠI DỊCH VỤ ITGOSHOP";
                sheet.Cells["A1:H1"].Style.Font.Bold = true;
                sheet.Cells["A1:H1"].Merge = true;
                //sheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["A2:C2"].Value = "Tầng 5, Số 117-119-121 Nguyễn Du, Phường Bến Thành, Quận 1, Thành Phố Hồ Chí Minh";
                sheet.Cells["A2:H2"].Merge = true;

                sheet.Cells["A3:H3"].Merge = true;

                sheet.Cells["A4:D4"].Value = "PHIẾU KIỂM KHO";
                sheet.Cells["A4:D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["A4:D4"].Style.Font.Bold = true;
                sheet.Cells["A4:D4"].Style.Font.Size = 16;
                sheet.Cells["A4:H4"].Merge = true;

                sheet.Cells["A5:H5"].Merge = true;
               
                sheet.Row(6).Height = 25;
                sheet.Cells["A6:H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["A6:H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells["A6:H6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells["A6:H6"].Style.Fill.BackgroundColor.SetColor(0, 186, 248, 255);
                sheet.Cells["A6:H6"].Style.Font.Bold = true;
                sheet.Cells["A6:H6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A6:H6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A6:H6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A6:H6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells["A6:A6"].Value = "Mã sản phẩm";
                sheet.Cells["B6:B6"].Value = "Tên sản phẩm";
                sheet.Cells["C6:C6"].Value = "Danh mục sản phẩm";
                sheet.Cells["D6:D6"].Value = "Thương hiệu";
                sheet.Cells["E6:E6"].Value = "Giá vốn";
                sheet.Cells["F6:F6"].Value = "Giá bán";
                sheet.Cells["G6:G6"].Value = "Đã bán";
                sheet.Cells["H6:H6"].Value = "Tồn kho khả dụng";

                ITGoShopContext context = HttpContext.RequestServices.GetService(typeof(ITGoShop_F_Ver2.Models.ITGoShopContext)) as ITGoShopContext;
                List<object> products = context.getAllProduct();

                int count = products.Count + 6;
                string productArea = $"A6:H{count}";
                sheet.Cells[productArea].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[productArea].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[productArea].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells[productArea].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                sheet.Cells[productArea].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[productArea].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int j = 7;
                foreach (var item in products)
                {
                    sheet.Row(j).Height = 25;
                    string masp = $"A{j}:A{j}";
                    string tensp = $"B{j}:B{j}";
                    string danhmucsp = $"C{j}:C{j}";
                    string thuonghieu = $"D{j}:D{j}";
                    string giavon = $"E{j}:E{j}";
                    string giaban = $"F{j}:F{j}";
                    string daban = $"G{j}:G{j}";
                    string tonkhokhadung = $"H{j}:H{j}";
                    sheet.Cells[masp].Value = item.GetType().GetProperty("ProductId").GetValue(item, null);
                    sheet.Cells[tensp].Value = item.GetType().GetProperty("ProductName").GetValue(item, null);
                    sheet.Cells[danhmucsp].Value = item.GetType().GetProperty("CategoryName").GetValue(item, null);
                    sheet.Cells[thuonghieu].Value = item.GetType().GetProperty("BrandName").GetValue(item, null);
                    sheet.Cells[giavon].Value = item.GetType().GetProperty("Cost").GetValue(item, null);
                    sheet.Cells[giaban].Value = item.GetType().GetProperty("Price").GetValue(item, null);
                    sheet.Cells[daban].Value = item.GetType().GetProperty("Sold").GetValue(item, null);
                    sheet.Cells[tonkhokhadung].Value = item.GetType().GetProperty("Quantity").GetValue(item, null);
                    j++;
                }
                sheet.Cells[$"G{count + 3}:G{count + 3}"].Value = "Ngày . . .  tháng . . . năm . . .";
                sheet.Cells[$"G{count + 3}:G{count + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells[$"G{count + 4}:G{count + 4}"].Value = "Nhân viên kiểm kho";
                sheet.Cells[$"G{count + 4}:G{count + 4}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"G{count + 4}:G{count + 4}"].Style.Font.Bold = true;

                sheet.Cells[$"G{count + 5}:G{count + 5}"].Value = "(Kí, ghi rõ họ tên)";
                sheet.Cells[$"G{count + 5}:G{count + 5}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; 
                sheet.Cells[$"G{count + 5}:G{count + 5}"].Style.Font.Italic = true;

                sheet.Cells[productArea].AutoFitColumns();
                sheet.Name = "Product";
                package.Save();
            }
            stream.Position = 0;

            var tenfile = $"tenfile.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", tenfile);
        }
    }
}
