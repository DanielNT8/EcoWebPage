using EcoBO.Models;
using EcoRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EcoDbContext _context;

        public UserRepository(EcoDbContext context)
        {
            _context = context;
        }
        public async Task<int> CountNewUsersAsync(DateTime from, DateTime to)
        {
            // Đếm user ĐĂNG KÝ MỚI trong khoảng thời gian này
            return await _context.Users
                .AsNoTracking()
                .CountAsync(u => u.CreatedAt >= from && u.CreatedAt <= to); // Giả sử User có CreatedAt
        }
    }
}
