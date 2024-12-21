using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ComputerStore.Areas.Admin.Controllers
{
    public class ProductDetailsController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();

        // GET: Admin/ProductDetails, tôi sửa cả index, dùng viewmodel
        public ActionResult Index()
        {
            var products = db.Products.Select(p => new ProductEditViewModel
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price,
                Quantity = p.Quantity,
                Description = p.Description,
                CategoryID = p.CategoryID,
                CategoryName = p.Category.CategoryName,  // Lấy tên danh mục
                Image1 = p.Image1,
                Image2 = p.Image2,
                Specifications = p.ProductDetails.Select(d => new ProductSpecViewModel
                {
                    SpecificationID = d.SpecificationID,
                    SpecificationName = d.ProductSpecification.SpecificationName,
                    SpecificationValue = d.SpecificationValue
                }).ToList()
            }).ToList();

            return View(products);
        }

        // GET: Admin/ProductDetails/Create
        public ActionResult Create()
        {
            // Lấy danh sách các Category từ bảng Categories
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/ProductDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase Image1)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có file ảnh không
                if (Image1 != null && Image1.ContentLength > 0)
                {
                    // Tạo tên file duy nhất
                    var fileName = Path.GetFileName(Image1.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/Products"), fileName);

                    // Lưu file vào thư mục
                    Image1.SaveAs(path);

                    // Gán đường dẫn file vào thuộc tính Image1
                    product.Image1 = "~/Images/Products/" + fileName;
                }

                // Lưu sản phẩm mới vào bảng Products
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Truyền lại danh sách Categories nếu lưu thất bại
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/ProductDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            #region code v1, lỗi nếu không có specs
            //if (id == null)
            //{
            //	return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            //var product = db.Products.Where(p => p.ProductID == id).Select(p => new ProductEditViewModel
            //{
            //	ProductID = p.ProductID,
            //	ProductName = p.ProductName,
            //	Price = p.Price,
            //	Quantity = p.Quantity,
            //	Description = p.Description,
            //	CategoryID = p.Category.CategoryID,
            //	CategoryName = p.Category.CategoryName,
            //	Image1 = p.Image1,
            //	Image2 = p.Image2,
            //	Specifications = db.ProductDetails
            //	.Where(d => d.ProductID == p.ProductID)
            //	.Select(d => new ProductSpecViewModel
            //	{
            //		SpecificationID = d.SpecificationID,
            //		SpecificationValue = d.SpecificationValue,
            //		SpecificationName = db.ProductSpecifications
            //			.Where(s => s.SpecificationID == d.SpecificationID)
            //			.Select(s => s.SpecificationName)
            //			.FirstOrDefault()
            //	}).ToList()
            //}).FirstOrDefault();


            //if (product == null)
            //{
            //	return HttpNotFound();
            //}

            //return View(product);
            #endregion

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy sản phẩm cần sửa
            var product = db.Products.Where(p => p.ProductID == id)
                .Select(p => new ProductEditViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Description = p.Description,
                    CategoryID = p.Category.CategoryID,
                    CategoryName = p.Category.CategoryName,
                    Image1 = p.Image1,
                    Image2 = p.Image2,
                    Specifications = db.ProductDetails
                        .Where(d => d.ProductID == p.ProductID)
                        .Select(d => new ProductSpecViewModel
                        {
                            SpecificationID = d.SpecificationID,
                            SpecificationValue = d.SpecificationValue,
                            SpecificationName = db.ProductSpecifications
                                .Where(s => s.SpecificationID == d.SpecificationID)
                                .Select(s => s.SpecificationName)
                                .FirstOrDefault()
                        }).ToList()
                }).FirstOrDefault();

            // Nếu không tìm thấy sản phẩm
            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy thông số kỹ thuật từ bảng ProductSpecifications theo CategoryID
            var categorySpecifications = db.ProductSpecifications
                .Where(s => s.CategoryID == product.CategoryID)
                .Select(s => new ProductSpecViewModel
                {
                    SpecificationID = s.SpecificationID,
                    SpecificationName = s.SpecificationName
                }).ToList();

            // Nếu sản phẩm chưa có thông số kỹ thuật, thêm các thông số của category vào model
            foreach (var spec in categorySpecifications)
            {
                if (!product.Specifications.Any(s => s.SpecificationID == spec.SpecificationID))
                {
                    product.Specifications.Add(spec);
                }
            }

            return View(product);
        }

        // POST: Admin/ProductDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductEditViewModel model)
        {
            try
            {
                var product = db.Products.Find(model.ProductID);

                if (product == null)
                {
                    return HttpNotFound();
                }

                product.ProductName = model.ProductName;
                product.Price = model.Price;
                product.Quantity = model.Quantity;
                product.Description = model.Description;
                product.CategoryID = model.CategoryID;
                //product.Image1 = model.Image1;
                //product.Image2 = model.Image2;
                #region Xử lý lưu file (không xử lý trùng tên file vì test để ghi đè luôn đỡ loạn file, folder)
                var file1 = Request.Files["ProductImage1"];
                if (file1 != null && file1.ContentLength > 0)
                {
                    string fileName1 = Path.GetFileName(file1.FileName);
                    string path1 = Server.MapPath("~/assets_user/image/data-image/" + fileName1);
                    file1.SaveAs(path1);
                    product.Image1 = fileName1;

                }

                var file2 = Request.Files["ProductImage2"];
                if (file2 != null && file2.ContentLength > 0)
                {
                    string fileName2 = Path.GetFileName(file2.FileName);
                    string path2 = Path.Combine(Server.MapPath("~/assets_user/image/data-image"), fileName2);
                    file2.SaveAs(path2);
                    product.Image2 = fileName2;
                }

                #endregion
                // Cập nhật thông số kỹ thuật
                foreach (var spec in model.Specifications)
                {
                    var detail = db.ProductDetails
                        .FirstOrDefault(d => d.ProductID == model.ProductID && d.SpecificationID == spec.SpecificationID);

                    if (detail != null)
                    {
                        detail.SpecificationValue = spec.SpecificationValue;
                        db.Entry(detail).State = EntityState.Modified;
                    }
                    else
                    {
                        // Nếu chưa có thông số kỹ thuật, thêm mới
                        db.ProductDetails.Add(new ProductDetail
                        {
                            ProductID = model.ProductID,
                            SpecificationID = spec.SpecificationID,
                            SpecificationValue = spec.SpecificationValue
                        });
                    }
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi cập nhật sản phẩm: " + ex.Message);
                return View(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Tạm thời tôi đặt class ở đây
    public class ProductEditViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }


        // Thêm danh sách thông số kỹ thuật
        public List<ProductSpecViewModel> Specifications { get; set; }
    }

    public class ProductSpecViewModel
    {
        public int SpecificationID { get; set; }
        public string SpecificationName { get; set; } // Tên thông số kỹ thuật
        public string SpecificationValue { get; set; } // Giá trị của thông số
    }

}
