using EcoBO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Contact
{
    public class ContactResponse
    {
        public Guid ContactId { get; set; }
        public string UserName { get; set; }
        public string? ContactInfo { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
