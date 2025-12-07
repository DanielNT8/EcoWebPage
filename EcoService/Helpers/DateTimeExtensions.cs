using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Helpers
{
    public static class DateTimeExtensions
    {
        // Hàm chuyển đổi chính cho DateTime? (có thể null)
        public static DateTime? ToVietnamTime(this DateTime? utcDateTime)
        {
            if (!utcDateTime.HasValue) return null;
            return ToVietnamTime(utcDateTime.Value);
        }

        // Hàm chuyển đổi cho DateTime (không null)
        public static DateTime ToVietnamTime(this DateTime utcDateTime)
        {
            // 1. Xác định ID múi giờ dựa trên Hệ điều hành (Quan trọng khi deploy)
            // Windows dùng: "SE Asia Standard Time"
            // Linux/Docker dùng: "Asia/Ho_Chi_Minh"
            string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "SE Asia Standard Time"
                : "Asia/Ho_Chi_Minh";

            try
            {
                // 2. Lấy thông tin TimeZone từ hệ thống
                TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                // 3. Thực hiện chuyển đổi từ UTC sang giờ VN
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback: Nếu server không tìm thấy timezone (hiếm gặp), cộng thủ công 7 tiếng
                return utcDateTime.AddHours(7);
            }
        }
    }
}
