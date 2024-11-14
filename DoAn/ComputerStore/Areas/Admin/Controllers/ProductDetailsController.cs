using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            var productDetails = db.ProductDetails.Include(p => p.Product).Include(p => p.ProductSpecification);
            return View(productDetails.ToList());
        }

        // GET: Admin/ProductDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

        // GET: Admin/ProductDetails/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.SpecificationID = new SelectList(db.ProductSpecifications, "SpecificationID", "SpecificationName");
            return View();
        }

        // POST: Admin/ProductDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,SpecificationID,SpecificationValue")] ProductDetail productDetail)
        {
            if (ModelState.IsValid)
            {
                db.ProductDetails.Add(productDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productDetail.ProductID);
            ViewBag.SpecificationID = new SelectList(db.ProductSpecifications, "SpecificationID", "SpecificationName", productDetail.SpecificationID);
            return View(productDetail);
        }

        // GET: Admin/ProductDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productDetail.ProductID);
            ViewBag.SpecificationID = new SelectList(db.ProductSpecifications, "SpecificationID", "SpecificationName", productDetail.SpecificationID);
            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Admin/ProductDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductDetail productDetail = db.ProductDetails.Find(id);
            db.ProductDetails.Remove(productDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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
