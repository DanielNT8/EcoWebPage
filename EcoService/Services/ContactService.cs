using EcoBO.Common;
using EcoBO.DTO.Contact;
using EcoBO.Enums;
using EcoBO.Models;
using EcoRepository.Interfaces;
using EcoService.Interfaces;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;

    public ContactService(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    // ✅ Create
    public async Task<ContactResponse> CreateAsync(ContactRequest request)
    {
        var contact = new Contact
        {
            UserName = request.UserName,
            ContactInfo = request.ContactInfo,
            Message = request.Message,
            Status = "Pending", // mặc định khi tạo
            CreatedAt = DateTime.UtcNow
        };

        await _contactRepository.AddAsync(contact);

        return new ContactResponse
        {
            ContactId = contact.Id,
            UserName = contact.UserName,
            ContactInfo = contact.ContactInfo,
            Message = contact.Message,
            Status = contact.Status,
            CreatedAt = contact.CreatedAt
        };
    }

    // ✅ Get All (filter + search + sort + paging)
    public async Task<PagedResult<ContactResponse>> GetAllAsync(ContactFilterParam filter)
    {
        var result = await _contactRepository.GetContactsAsync(filter);

        var mapped = result.Items.Select(c => new ContactResponse
        {
            ContactId = c.Id,
            UserName = c.UserName,
            ContactInfo = c.ContactInfo,
            Message = c.Message,
            Status = c.Status,
            CreatedAt = c.CreatedAt
        }).ToList();

        return new PagedResult<ContactResponse>(mapped, result.TotalCount, result.PageIndex, result.PageSize);
    }

    // ✅ Get by ID
    public async Task<ContactResponse?> GetByIdAsync(Guid id)
    {
        var contact = await _contactRepository.GetByIdAsync(id);
        if (contact == null) return null;

        return new ContactResponse
        {
            ContactId = contact.Id,
            UserName = contact.UserName,
            ContactInfo = contact.ContactInfo,
            Message = contact.Message,
            Status = contact.Status,
            CreatedAt = contact.CreatedAt
        };
    }

    // ✅ Update status
    public async Task<ContactResponse?> UpdateStatusAsync(Guid contactId, ContactStatus newStatus)
    {
        var contact = await _contactRepository.GetByIdAsync(contactId);
        if (contact == null) return null;

        contact.Status = newStatus.ToString();
        contact.UpdatedAt = DateTime.UtcNow;

        await _contactRepository.UpdateAsync(contact);

        return new ContactResponse
        {
            ContactId = contact.Id,
            UserName = contact.UserName,
            ContactInfo = contact.ContactInfo,
            Message = contact.Message,
            Status = contact.Status,
            CreatedAt = contact.CreatedAt
        };
    }

    // ✅ Delete
    public async Task<bool> DeleteAsync(Guid contactId)
    {
        var contact = await _contactRepository.GetByIdAsync(contactId);
        if (contact == null) return false;

        await _contactRepository.DeleteAsync(contact);
        return true;
    }
}