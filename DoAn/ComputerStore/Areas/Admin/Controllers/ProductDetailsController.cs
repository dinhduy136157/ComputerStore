using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;

namespace ComputerStore.Areas.Admin.Controllers
{
    public class ProductDetailsController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();

        // GET: Admin/ProductDetails
        public ActionResult Index()
        {
            var productDetails = db.ProductDetails.Include(p => p.Product).Include(p => p.Product.Category).Include(p => p.ProductSpecification);
            return View(productDetails.ToList());
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productDetail = db.ProductDetails.FirstOrDefault(pd => pd.ProductID == id);

            if (productDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productDetail.ProductID);
            ViewBag.SpecificationID = new SelectList(db.ProductSpecifications, "SpecificationID", "SpecificationName", productDetail.SpecificationID);
            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,SpecificationID,SpecificationValue")] ProductDetail productDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productDetail.ProductID);
            ViewBag.SpecificationID = new SelectList(db.ProductSpecifications, "SpecificationID", "SpecificationName", productDetail.SpecificationID);
            return View(productDetail);
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
}
