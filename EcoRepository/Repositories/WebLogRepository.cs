using EcoBO.Models;
using EcoRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Repositories
{
    public class WebLogRepository : IWebLogRepository
    {
        private readonly EcoDbContext _context;
        public WebLogRepository(EcoDbContext context) => _context = context;

        public async Task AddLogAsync(WebLog log)
        {
            _context.WebLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
