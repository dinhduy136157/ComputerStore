﻿$(document).ready(function () {
    var o, e = function () {
        // Ví dụ: sử dụng dữ liệu để hiển thị đồ thị hoặc thống kê
        var statusOrderCounts = statusCounts.map(function (item) {
            return item.OrderCount; // Lấy số lượng OrderCount từ mỗi item
        });

        var categoryOrderCounts = categoryCounts.map(function (item) {
            return item.OrderCount; // Lấy số lượng OrderCount từ mỗi item
        });

        var totalRevenue = monthlyStatistics.forEach(function (item) {
            return item.TotalRevenue;
        });
        var totalQuantitySold = monthlyStatistics.forEach(function (item) {
            return item.Total;
        });

        $("#sparkline1").sparkline([110, 23, 43, 35, 44, 45, 56, 37, 40], {
            type: "line",
            width: "100%",
            height: 210,
            chartRangeMax: 50,
            lineColor: "#7266ba",
            fillColor: "rgba(114,102,186,0.3)",
            highlightLineColor: "rgba(0,0,0,.1)",
            highlightSpotColor: "rgba(0,0,0,.2)",
            maxSpotColor: !1,
            minSpotColor: !1,
            spotColor: !1,
            lineWidth: 1
        }), $("#sparkline1").sparkline([25, 23, 26, 24, 25, 32, 30, 24, 19], {
            type: "line",
            width: "100%",
            height: "210",
            chartRangeMax: 40,
            lineColor: "#f672a7",
            fillColor: "rgba(246, 114, 164, 0.3)",
            composite: !0,
            highlightLineColor: "rgba(0,0,0,.1)",
            highlightSpotColor: "rgba(0,0,0,.2)",
            maxSpotColor: !1,
            minSpotColor: !1,
            spotColor: !1,
            lineWidth: 1
        }), $("#sparkline2").sparkline([
            [170, 140],
            [190, 50],
            [100, 150],
            [140, 80],
            [50, 90],
            [80, 120],
            [130, 80],
            [90, 70],
            [80, 50],
            [120, 130],
            [120, 100],
            [140, 110]
        ], {
            type: "bar",
            height: "210",
            barWidth: "15",
            barSpacing: "3",
            stackedBarColor: ["#7266ba", "#e3eaef"]
        }), $("#sparkline3").sparkline([categoryOrderCounts[0], categoryOrderCounts[1], categoryOrderCounts[2], categoryOrderCounts[3]], {
            type: "pie",
            width: "210",
            height: "210",
            sliceColors: ["#e3eaef", "#7266ba", "#f672a7", "#f672a7"]
        })
    };
    e(), $(window).resize(function (i) {
        clearTimeout(o), o = setTimeout(function () {
            e()
        }, 300)
    })
});