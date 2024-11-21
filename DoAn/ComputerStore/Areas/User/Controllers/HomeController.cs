using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;
using System.Data.Entity;


namespace ComputerStore.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();
        public class ProductDetailDto
        {
            public ProductDetail Pd { get; set; }
            public Product P { get; set; }
        }

        public class ProductDetailsViewModel
        {
            public List<ProductDetailDto> PcDetailsList { get; set; }
            public List<ProductDetailDto> PhoneDetailsList { get; set; }
            public List<ProductDetailDto> ScreenDetailsList { get; set; }
        }


        public ActionResult Index()
        {
            // Danh sách 10 PC từ top10PCIds
            var top10PCIds = (from oi in db.OrderItems
                              join p in db.Products on oi.ProductID equals p.ProductID
                              where p.CategoryID == 1
                              group oi by oi.ProductID into g
                              orderby g.Sum(x => x.Quantity) descending
                              select g.Key)
                                   .Take(10)
                                   .ToList();

            // Lấy chi tiết sản phẩm từ ProductDetails với 10 ProductID này
            var pcDetailsList = (from pd in db.ProductDetails
                                 join p in db.Products on pd.ProductID equals p.ProductID
                                 where top10PCIds.Contains(pd.ProductID)
                                 select new ProductDetailDto
                                 {
                                     Pd = pd,
                                     P = p
                                 }).ToList();  // Trả về danh sách ProductDetailDto
            // Danh sách 10 Tai nghe từ top10PhoneIds
            var top10PhoneIds = (from oi in db.OrderItems
                                 join p in db.Products on oi.ProductID equals p.ProductID
                                 where p.CategoryID == 2
                                 group oi by oi.ProductID into g
                                 orderby g.Sum(x => x.Quantity) descending
                                 select g.Key)
                                   .Take(10)
                                   .ToList();

            // Lấy chi tiết sản phẩm từ top10PhoneIds với 10 ProductID này
            var headphoneDetailsList = (from pd in db.ProductDetails
                                        join p in db.Products on pd.ProductID equals p.ProductID
                                        where top10PhoneIds.Contains(pd.ProductID)
                                        select new ProductDetailDto
                                        {
                                            Pd = pd,
                                            P = p
                                        }).ToList();  // Trả về danh sách ProductDetailDto

            // Danh sách 10 Màn hình từ top10Screen
            var top10Screen = (from oi in db.OrderItems
                               join p in db.Products on oi.ProductID equals p.ProductID
                               where p.CategoryID == 3
                               group oi by oi.ProductID into g
                               orderby g.Sum(x => x.Quantity) descending
                               select g.Key)
                                   .Take(10)
                                   .ToList();

            // Lấy chi tiết sản phẩm từ top10Screen với 10 ProductID này
            var screenDetailsList = (from pd in db.ProductDetails
                                     join p in db.Products on pd.ProductID equals p.ProductID
                                     where top10Screen.Contains(pd.ProductID)
                                     select new ProductDetailDto
                                     {
                                         Pd = pd,
                                         P = p
                                     }).ToList();  // Trả về danh sách ProductDetailDto

            // Tạo ViewModel và truyền các danh sách chi tiết vào
            var viewModel = new ProductDetailsViewModel
            {
                PcDetailsList = pcDetailsList,
                PhoneDetailsList = headphoneDetailsList,
                ScreenDetailsList = screenDetailsList
            };

            // Trả về View với ViewModel
            return View(viewModel);
        }

    }
}