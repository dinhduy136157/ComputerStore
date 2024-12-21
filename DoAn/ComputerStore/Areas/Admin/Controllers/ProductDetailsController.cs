using System;
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
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
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
                    try
                    {
                        // Tạo tên file duy nhất
                        var fileName = Path.GetFileName(Image1.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/Products"), fileName);

                        // Kiểm tra nếu thư mục không tồn tại thì tạo mới
                        var directory = Path.GetDirectoryName(path);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // Lưu file vào thư mục
                        Image1.SaveAs(path);

                        // Gán đường dẫn file vào thuộc tính Image1
                        product.Image1 = "~/Images/Products/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi khi tải ảnh
                        ModelState.AddModelError("", "Lỗi khi tải ảnh: " + ex.Message);
                        return View(product);
                    }
                }

                try
                {
                    // Lưu sản phẩm mới vào bảng Products
                    db.Products.Add(product);
                    db.SaveChanges();

                    // Chuyển hướng về trang danh sách sản phẩm
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi khi lưu sản phẩm
                    ModelState.AddModelError("", "Lỗi khi lưu sản phẩm: " + ex.Message);
                }
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

            // Truyền các dữ liệu cần thiết cho View
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
                try
                {
                    db.Entry(productDetail).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi khi sửa thông tin sản phẩm
                    ModelState.AddModelError("", "Lỗi khi sửa sản phẩm: " + ex.Message);
                }
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productDetail.ProductID);
            ViewBag.SpecificationID = new SelectList(db.ProductSpecifications, "SpecificationID", "SpecificationName", productDetail.SpecificationID);
            return View(productDetail);
        }

        // Phương thức Dispose để giải phóng tài nguyên
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
