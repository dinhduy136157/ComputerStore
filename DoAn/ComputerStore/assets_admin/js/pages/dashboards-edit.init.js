$(document).ready(function () {
    var o, e = function () {
        // Ví dụ: sử dụng dữ liệu để hiển thị đồ thị hoặc thống kê
        var statusOrderCounts = statusCounts.map(function (item) {
            return item.OrderCount; // Lấy số lượng OrderCount từ mỗi item
        });

        var categoryOrderCounts = categoryCounts.map(function (item) {
            return item.OrderCount; // Lấy số lượng OrderCount từ mỗi item
        });

        var totalRevenue = monthlyStatistics.map(function (item) {
            return item.TotalRevenue;
        });
        var totalQuantitySold = monthlyStatistics.map(function (item) {
            return item.TotalQuantitySold;
        });
        console.log(totalRevenue);
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
            [totalQuantitySold[0], totalRevenue[0]],
            [totalQuantitySold[1], totalRevenue[1]],
            [totalQuantitySold[2], totalRevenue[2]],
            [totalQuantitySold[3], totalRevenue[3]],
            [totalQuantitySold[4], totalRevenue[4]],
            [totalQuantitySold[5], totalRevenue[5]],
            [totalQuantitySold[6], totalRevenue[6]],
            [totalQuantitySold[7], totalRevenue[7]],
            [totalQuantitySold[8], totalRevenue[8]],
            [totalQuantitySold[9], totalRevenue[9]],
            [totalQuantitySold[10], totalRevenue[10]],
            [totalQuantitySold[11], totalRevenue[11]]
        ], {
            type: "bar",
            height: "210",
            barWidth: "15",
            barSpacing: "3",
            stackedBarColor: ["#e3eaef", "#7266ba"]
        }), $("#sparkline3").sparkline([categoryOrderCounts[0], categoryOrderCounts[1], categoryOrderCounts[2], categoryOrderCounts[3]], {
            type: "pie",
            width: "210",
            height: "210",
            sliceColors: ["#e3eaef", "#7266ba", "#f672a7", "green"]
        })
    };
    e(), $(window).resize(function (i) {
        clearTimeout(o), o = setTimeout(function () {
            e()
        }, 300)
    })
});