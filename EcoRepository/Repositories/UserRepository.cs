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
        public async Task<int> CountUsersAsync()
        {
            return await _context.Users.CountAsync(u => u.DeletedAt == null); // Chỉ đếm user chưa xóa
        }
    }
}
