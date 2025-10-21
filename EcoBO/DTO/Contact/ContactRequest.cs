using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Contact
{
    public class ContactRequest
    {
        public string UserName { get; set; }
        public string? ContactInfo { get; set; }
        public string? Message { get; set; }
    }
}
