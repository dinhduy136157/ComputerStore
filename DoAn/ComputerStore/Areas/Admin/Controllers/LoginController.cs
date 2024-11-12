using ComputerStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComputerStore.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();

        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // Tìm người dùng trong cơ sở dữ liệu
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Nếu tìm thấy người dùng và mật khẩu đúng, lưu vào session và điều hướng
                Session["UserEmail"] = email;
                Session["UserId"] = user.UserID;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Nếu không tìm thấy, hiển thị lỗi
                ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng!";
                return View("Index");
            }
        }

    }
}