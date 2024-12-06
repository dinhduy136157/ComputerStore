using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;
using System.Data.Entity;
using static ComputerStore.Areas.User.Controllers.HomeController;

namespace ComputerStore.Areas.User.Controllers
{


    public class CategoriesController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();
        public ActionResult Card3060()
        {
            var data = (from pd in db.ProductDetails
                        join p in db.Products on pd.ProductID equals p.ProductID
                        join ps in db.ProductSpecifications on pd.SpecificationID equals ps.SpecificationID
                        where p.CategoryID == 1 && pd.SpecificationValue == "CARD GTX 3060"
                        select pd).ToList();
            return View(data);
        }
        public ActionResult ManHinhAsusTuf()
        {
            var data = (from pd in db.ProductDetails
                        join p in db.Products on pd.ProductID equals p.ProductID
                        join ps in db.ProductSpecifications on pd.SpecificationID equals ps.SpecificationID
                        where p.CategoryID == 2 && pd.SpecificationValue == "Asus TUF"
                        select pd).ToList();
            return View(data);
        }
    }

}
