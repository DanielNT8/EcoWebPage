using EcoBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Interfaces
{
    public interface IWebLogRepository
    {
        Task AddLogAsync(WebLog log);
    }
}
