using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComputerStore.Areas.User.Controllers
{
    public class OrderItemsController : Controller
    {
        // GET: User/OrderItems
        public ActionResult Index()
        {
            return View();
        }
    }
}