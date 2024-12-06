using System;
using System.Linq;
using System.Web.Mvc;
using ComputerStore.Models;
using System.Data.Entity;
namespace ComputerStore.Areas.User.Controllers
{
    public class ProductDetailsController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();

        // GET: User/ProductDetails/Details/5
        public ActionResult Index(int id)
        {
            // Tìm sản phẩm theo id
            var productDetails = db.ProductDetails
            .Where(pd => pd.ProductID == id)
            .Include(pd => pd.Product)
            .Include(pd => pd.Product.Category)
            .Include(pd => pd.ProductSpecification)
            .ToList();

            // Kiểm tra nếu sản phẩm không tồn tại
            if (productDetails == null)
            {
                return HttpNotFound("Product not found.");
            }

            // Trả về view với thông tin sản phẩm
            return View(productDetails);
        }
    }
}
