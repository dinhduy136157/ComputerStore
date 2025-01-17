﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComputerStore.Models;

namespace ComputerStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private ComputerStoreEntities db = new ComputerStoreEntities();
        public class DashboardViewModel
        {
            public List<StatusCount> StatusCounts { get; set; }
            public List<CategoryCount> CategoryCounts { get; set; }
            public int totalStatusOrderCount { get; set; }
            public int totalCategoryOrderCount { get; set; }
            public List<MonthlyStatistics> MonthlyStatistics { get; set; } // Thêm danh sách theo tháng

        }
        public class MonthlyStatistics
        {
            public int Month { get; set; }
            public int TotalQuantitySold { get; set; }
            public decimal TotalRevenue { get; set; }
        }
        public class StatusCount
        {
            public string Status { get; set; }
            public int OrderCount { get; set; }
        }

        public class CategoryCount
        {
            public string CategoryName { get; set; }
            public int OrderCount { get; set; }
        }


        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // Truy vấn thống kê theo Status
            var statusCounts = db.OrderItems
                .Where(o => o.Status == "Đang chờ xác nhận" ||
                            o.Status == "Đã hủy" ||
                            o.Status == "Giao hàng thành công")
                .GroupBy(o => o.Status)
                .Select(g => new StatusCount
                {
                    Status = g.Key,
                    OrderCount = g.Count()
                })
                .ToList();

            // Truy vấn thống kê theo Category
            var categoryCounts = (from o in db.OrderItems
                                  join p in db.Products on o.ProductID equals p.ProductID
                                  join c in db.Categories on p.CategoryID equals c.CategoryID
                                  where new[] { "Cây máy tính", "Màn hình", "Tai nghe", "Chuột" }.Contains(c.CategoryName)
                                  group o by c.CategoryName into g
                                  select new CategoryCount
                                  {
                                      CategoryName = g.Key,
                                      OrderCount = g.Count()
                                  }).ToList();
            var totalStatusOrderCount = statusCounts.Sum(s => s.OrderCount);
            var totalCategoryOrderCount = categoryCounts.Sum(s => s.OrderCount);



            var query = from o in db.Orders
                        join oi in db.OrderItems on o.OrderID equals oi.OrderID
                        where o.OrderDate.Year == 2024
                        group new { oi.Quantity, oi.Price } by o.OrderDate.Month into g
                        orderby g.Key
                        select new MonthlyStatistics
                        {
                            Month = g.Key,
                            TotalQuantitySold = g.Sum(x => x.Quantity), // Tổng số lượng bán
                            TotalRevenue = g.Sum(x => x.Quantity * x.Price) // Tổng doanh thu
                        };

            var monthlyData = query.ToList(); // Lấy danh sách dữ liệu theo từng tháng


            var viewModel = new DashboardViewModel
            {
                StatusCounts = statusCounts, // Dữ liệu khác bạn đã có
                CategoryCounts = categoryCounts, // Dữ liệu khác bạn đã có
                totalStatusOrderCount = totalStatusOrderCount, // Dữ liệu khác bạn đã có
                totalCategoryOrderCount = totalCategoryOrderCount, // Dữ liệu khác bạn đã có
                MonthlyStatistics = monthlyData // Gán danh sách theo tháng
            };

            return View(viewModel);

        }


    }
}