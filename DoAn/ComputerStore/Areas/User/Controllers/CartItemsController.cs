using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;
using System.Security.Cryptography;
using ComputerStore.Areas.User.Helpers;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;


namespace ComputerStore.Areas.User.Controllers
{
    public class CartItemsController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();

        // GET: User/CartItems
        public class CartItemViewModel
        {
            public CartItem CartItem { get; set; }
            public Cart Cart { get; set; }
            public Product Product { get; set; }
            public List<ProductDetail> ProductDetails { get; set; }
        }

        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserID"];

            var cartItems = (from ci in db.CartItems
                             join c in db.Carts on ci.CartID equals c.CartID
                             join p in db.Products on ci.ProductID equals p.ProductID
                             where c.UserID == userId
                             select new CartItemViewModel
                             {
                                 CartItem = ci,
                                 Cart = c,
                                 Product = p,
                                 ProductDetails = db.ProductDetails
                                                   .Where(pd => pd.ProductID == p.ProductID)
                                                   .ToList() // Lấy danh sách ProductDetails cho từng sản phẩm
                             }).ToList();

            return View(cartItems);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang Login
            }
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

        private string vnp_TmnCode = "MV4XNH7A";
        private string vnp_HashSecret = "LF3AFG07RJ3Z4Z4K6KFDVSY21SWO1VL5";


        public static string CreatePaymentUrl(long orderId, decimal amount, string returnUrl)
        {
            string vnp_TmnCode = "MV4XNH7A";
            string vnp_HashSecret = "LF3AFG07RJ3Z4Z4K6KFDVSY21SWO1VL5";
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

            SortedList<string, string> vnp_Params = new SortedList<string, string>
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", vnp_TmnCode },
        { "vnp_Amount", ((int)(amount * 100)).ToString() }, // Nhân với 100
        { "vnp_CurrCode", "VND" },
        { "vnp_TxnRef", orderId.ToString() },
        { "vnp_OrderInfo", "Thanh toan don hang #" + orderId },
        { "vnp_Locale", "vn" },
        { "vnp_ReturnUrl", returnUrl },
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
    };

            // In thông tin các tham số ra Output Window
            foreach (var param in vnp_Params)
            {
                System.Diagnostics.Debug.WriteLine($"{param.Key}: {param.Value}");
            }


            StringBuilder query = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in vnp_Params)
            {
                query.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            string rawHash = query.ToString().TrimEnd('&');
            string vnp_SecureHash = VnpayHelper.ComputeHmacSHA512(vnp_HashSecret, rawHash);
            System.Diagnostics.Debug.WriteLine($"{vnp_Url}?{query}vnpSecureHash={vnp_SecureHash}");
            return vnp_Url + "?" + query + "vnp_SecureHash=" + vnp_SecureHash;
        }


        [HttpPost]
        public ActionResult ThanhToan()
        {
            if (Session["UserID"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thanh toán." });
            }

            int userId = (int)Session["UserID"];
            var cartItems = db.CartItems.Where(ci => ci.Cart.UserID == userId).ToList();
            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống." });
            }

            decimal totalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price);
            long orderId = DateTime.Now.Ticks; // Mã đơn hàng duy nhất

            // Tạo URL thanh toán qua VNPay
            string returnUrl = Url.Action("PaymentCallback", "CartItems", null, Request.Url.Scheme);
            string paymentUrl = CreatePaymentUrl(orderId, totalAmount, returnUrl); // Gọi hàm CreatePaymentUrl

            return Json(new { success = true, paymentUrl });
        }


        // Hàm tính toán SecureHash
        public static string ComputeHmacSHA512(string key, string data)
        {
            using (HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }
        [HttpGet]
        public ActionResult PaymentCallback()
        {
            // Lấy các tham số trả về từ VNPay
            var vnp_Params = Request.QueryString;

            // Kiểm tra và tính toán lại SecureHash từ các tham số
            string vnp_SecureHash = vnp_Params["vnp_SecureHash"];
            vnp_Params.Remove("vnp_SecureHash");

            string rawHash = string.Join("&", vnp_Params.AllKeys.Select(k => k + "=" + vnp_Params[k]));
            string computedHash = VnpayHelper.ComputeHmacSHA512(vnp_HashSecret, rawHash);

            if (computedHash == vnp_SecureHash)
            {
                // Thực hiện xử lý theo kết quả
                string responseCode = vnp_Params["vnp_ResponseCode"];
                if (responseCode == "00")
                {
                    return Content("Giao dịch thành công");
                }
                else
                {
                    return Content("Giao dịch thất bại");
                }
            }
            else
            {
                return Content("Lỗi xác thực chữ ký.");
            }
        }

    }
}

