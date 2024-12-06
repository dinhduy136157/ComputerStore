using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComputerStore.Areas.User.Controllers
{
    public class TestProductDetailsController : Controller
    {
        // GET: User/TestProductDetails
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult GioHang()
		{
			return View();
		}

		public ActionResult ThanhToan()
		{
			return View();
		}

		public ActionResult QuanLyDonHang()
		{
			return View();
		}

		[HttpGet]
		public ActionResult OrderedItems()
		{
			return PartialView();
		}

		[HttpGet]
		public ActionResult AccountInfo()
		{
			return PartialView();
		}

		[HttpGet]
		public ActionResult AddressList()
		{
			return PartialView();
		}
	}
}