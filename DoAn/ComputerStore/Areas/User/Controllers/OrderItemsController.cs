using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data.Entity;

using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;

namespace ComputerStore.Areas.User.Controllers
{
    public class OrderItemsController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();


        // GET: User/OrderItems
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        [HttpGet]
        public ActionResult OrderedItems()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int userID = (int)Session["UserID"];
            var orderItems = db.OrderItems
            .Include(o => o.Order)
            .Include(o => o.Product)
            .Where(o => o.Order.UserID == userID) // Thêm điều kiện UserID
            .ToList();

            return PartialView(orderItems);
        }

        [HttpGet]
        public ActionResult AccountInfo()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult AddressList()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return PartialView();
        }
        public ActionResult CreateOrderWithItems()
        {
            try
            {
                // Lấy UserID từ session
                int userId = (int)Session["UserID"];

                // Lấy tất cả giỏ hàng của người dùng (nếu có nhiều giỏ hàng)
                var carts = db.Carts.Where(c => c.UserID == userId).ToList();

                if (carts == null || carts.Count == 0)
                {
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng của bạn." }, JsonRequestBehavior.AllowGet);
                }

                // Lặp qua từng giỏ hàng của người dùng
                foreach (var cart in carts)
                {
                    // Lấy tất cả các sản phẩm trong giỏ hàng hiện tại
                    var cartItems = db.CartItems.Where(ci => ci.CartID == cart.CartID).ToList();

                    if (cartItems.Count == 0)
                    {
                        continue; // Nếu giỏ hàng không có sản phẩm thì bỏ qua
                    }

                    // Tạo đơn hàng (Order)
                    var order = new Order
                    {
                        UserID = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price)
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();

                    // Thêm các sản phẩm vào OrderItems
                    foreach (var cartItem in cartItems)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderID = order.OrderID, // Liên kết với đơn hàng đã tạo
                            ProductID = cartItem.ProductID,
                            Quantity = cartItem.Quantity,
                            Price = cartItem.Product.Price,
                            Status = "Đang chờ xác nhận" // Hoặc bạn có thể set trạng thái khác
                        };
                        db.OrderItems.Add(orderItem);
                    }

                    // Lưu tất cả OrderItems vào cơ sở dữ liệu
                    db.SaveChanges();

                    // Xóa sản phẩm khỏi giỏ hàng sau khi đã đặt hàng
                    db.CartItems.RemoveRange(cartItems);
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Đặt hàng thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log lỗi
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { success = false, message = "Có lỗi xảy ra khi xử lý đặt hàng. Vui lòng thử lại." }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}