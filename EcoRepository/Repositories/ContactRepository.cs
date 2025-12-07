using EcoBO.Common;
using EcoBO.DTO.Contact;
using EcoBO.Enums;
using EcoBO.Models;
using EcoRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EcoRepository.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly EcoDbContext _context;

        public ContactRepository(EcoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Contact contact)
        {
            contact.Id = Guid.NewGuid();
            contact.CreatedAt = DateTime.Now;
            contact.Status = "Pending";
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
        }

        public async Task<Contact?> GetByIdAsync(Guid id)
        {
            return await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PagedResult<Contact>> GetContactsAsync(ContactFilterParam filter)
        {
            var query = _context.Contacts.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(c =>
                    (c.Message != null && c.Message.Contains(filter.Search)) ||
                    (c.ContactInfo != null && c.ContactInfo.Contains(filter.Search)) ||
                    (c.UserName != null && c.UserName.Contains(filter.Search)));
            }

            // Filter by Status
            if (!string.IsNullOrEmpty(filter.Status))
            {
                string statusLower = filter.Status.ToLower();
                query = query.Where(c => c.Status != null && c.Status.ToLower() == statusLower);
            }

            // Sort
            query = filter.SortBy?.ToLower() switch
            {
                "username" => filter.SortAscending ? query.OrderBy(c => c.UserName) : query.OrderByDescending(c => c.UserName),
                "message" => filter.SortAscending ? query.OrderBy(c => c.Message) : query.OrderByDescending(c => c.Message),
                "contactinfo" => filter.SortAscending ? query.OrderBy(c => c.ContactInfo) : query.OrderByDescending(c => c.ContactInfo),
                _ => filter.SortAscending ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
            };

            // Paging
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<Contact>(items, totalItems, filter.PageNumber, filter.PageSize);
        }

        public async Task UpdateAsync(Contact contact)
        {
            contact.UpdatedAt = DateTime.Now;
            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Contact contact)
        {
            contact.Status = "Deleted";
            contact.DeletedAt = DateTime.Now;
            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountContactsByStatusAsync(string status)
        {
      
            return await _context.Contacts.CountAsync(c => c.Status.ToLower() == status.ToLower());
        }

    }
}
