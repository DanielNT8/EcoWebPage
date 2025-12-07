using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoBO.DTO.Community.Response
{
    public class HashtagDto
    {
        public Guid Id { get; set; }
        public string TagName { get; set; }
        public int UsageCount { get; set; }
    }
}
