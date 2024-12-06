
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

using ComputerStoreUser = ComputerStore.Models.User;

namespace ComputerStore.Areas.User.Controllers
{
    public class AccountController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();


        // GET: User/Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ email và mật khẩu";
                    return View();
                }
                var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
                if (user != null)
                {
                    Session["UserID"] = user.UserID;
                    Session["UserName"] = user.UserName;
                    Session["Address"] = string.IsNullOrEmpty(user.Address) ? "Địa chỉ chưa có" : user.Address;
                    Session["Phone"] = string.IsNullOrEmpty(user.Phone) ? "Số điện thoại chưa có" : user.Phone;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng";
                }

            }
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ComputerStoreUser newuser)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại
                var existingUser = db.Users.FirstOrDefault(u => u.Email == newuser.Email);

                if (existingUser == null)
                {
                    db.Users.Add(newuser);
                    db.SaveChanges();

                    
                    Session["UserID"] = newuser.UserID;
                    Session["UserName"] = newuser.UserName;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                 
                    ViewBag.ErrorMessage = "Email đã được sử dụng. Vui lòng chọn email khác!";
                }
            }
            return View(newuser);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

    }
}
