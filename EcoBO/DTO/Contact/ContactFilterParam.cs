using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Contact
{
    public class ContactFilterParam
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; } = "createdAt";
        public bool SortAscending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
