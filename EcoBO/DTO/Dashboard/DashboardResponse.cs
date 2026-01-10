using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Dashboard
{
    // 2. Response: Cấu trúc dữ liệu trả về cho FE
    public class DashboardResponse
    {
        // Các thẻ Card thống kê (kèm % tăng trưởng)
        public MetricItem TotalRevenue { get; set; }
        public MetricItem NewUsers { get; set; }
        public MetricItem ProcessedContacts { get; set; }

        // Dữ liệu biểu đồ
        public List<ChartDataPoint> RevenueChart { get; set; }
    }

    // Class phụ để hiển thị con số + % thay đổi (Ví dụ: 10tr (+5%))
    public class MetricItem
    {
        public double Value { get; set; }
        public double PreviousValue { get; set; } // Giá trị kỳ trước để so sánh
        public double GrowthPercent // Công thức tính %
        {
            get
            {
                if (PreviousValue == 0) return Value > 0 ? 100 : 0;
                return Math.Round(((Value - PreviousValue) / PreviousValue) * 100, 2);
            }
        }
    }

    // Class phụ cho biểu đồ
    public class ChartDataPoint
    {
        public string Label { get; set; } // "01/01" hoặc "Jan 2024"
        public DateTime Date { get; set; } // Để sort
        public double Value { get; set; }
    }
}
