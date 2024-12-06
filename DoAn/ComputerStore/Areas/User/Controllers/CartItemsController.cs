using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;

namespace ComputerStore.Areas.User.Controllers
{
    public class CartItemsController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();

        // GET: User/CartItems
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang Login
            }
            var userId = (int)Session["UserID"];
            var cartItems = (from ci in db.CartItems
                             join c in db.Carts on ci.CartID equals c.CartID
                             join p in db.Products on ci.ProductID equals p.ProductID
                             join pd in db.ProductDetails on p.ProductID equals pd.ProductID
                             where c.UserID == 1
                             select ci).ToList();  // Chỉ chọn CartItem


            return View(cartItems.ToList());
        }
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            int userId = (int)Session["UserID"];


            // Kiểm tra sản phẩm
            var product = db.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Xử lý thêm vào giỏ hàng
            var cartItem = (from ci in db.CartItems
                            join c in db.Carts on ci.CartID equals c.CartID
                            where c.UserID == userId && ci.ProductID == productId
                            select new
                            {
                                CartItem = ci,
                                Cart = c
                            }).FirstOrDefault();
            if (cartItem != null)
            {
                quantity += quantity;
            }
            else
            {
                // Thêm mới Cart
                var newCart = new Cart
                {
                    UserID = userId,
                    CreatedDate = DateTime.Now
                };

                db.Carts.Add(newCart);
                db.SaveChanges(); 

                // Thêm mới CartItem và gán CartID cho nó
                var newCartItem = new CartItem
                {
                    CartID = newCart.CartID,  
                    ProductID = productId,
                    Quantity = quantity
                };

                db.CartItems.Add(newCartItem);
                db.SaveChanges(); 

            }
            return Json(new { success = true, message = "Thêm vào giỏ hàng thành công!" });
        }

        [HttpPost]
        public ActionResult BuyNow(int productId, int quantity)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang Login
            }
            // Lưu vào giỏ hàng
            AddToCart(productId, quantity);

            // Điều hướng đến trang giỏ hàng
            return RedirectToAction("Index", "Cart");
        }


    }
}
