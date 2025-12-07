using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.Common
{
    public class BaseFilterRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; } // Tìm kiếm chung
        public string? SortBy { get; set; } // Sắp xếp theo trường nào
        public bool IsDescending { get; set; } = true;
    }
}
