using EcoBO.Common;
using EcoBO.DTO.Contact;
using EcoBO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoService.Interfaces
{
    public interface IContactService
    {
        Task<ContactResponse> CreateAsync(ContactRequest request);
        Task<PagedResult<ContactResponse>> GetAllAsync(ContactFilterParam filter);
        Task<ContactResponse?> GetByIdAsync(Guid id);
        Task<ContactResponse?> UpdateStatusAsync(Guid contactId, ContactStatus newStatus);
        Task<bool> DeleteAsync(Guid contactId);
    }
}
