using EcoBO.Common;
using EcoBO.DTO.Contact;
using EcoBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoRepository.Interfaces
{
    public interface IContactRepository
    {
        Task AddAsync(Contact contact);
        Task<Contact?> GetByIdAsync(Guid id);
        Task<PagedResult<Contact>> GetContactsAsync(ContactFilterParam filter);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(Contact contact);
        Task<int> CountContactsByStatusAsync(string status);
    }
}
