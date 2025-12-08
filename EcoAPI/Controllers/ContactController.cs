using EcoBO.DTO.Contact;
using EcoBO.Enums;
using EcoService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EcoAPI.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // ✅ Create new contact
        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] ContactRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest(new { Message = "UserName is required" });

            var result = await _contactService.CreateAsync(request);
            return Ok(result);
        }

        // ✅ Get all contacts
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetContacts([FromQuery] ContactFilterParam filter)
        {
            var result = await _contactService.GetAllAsync(filter);
            return Ok(result);
        }

        // ✅ Get contact by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(Guid id)
        {
            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound(new { Message = "Contact not found" });

            return Ok(contact);
        }

        // ✅ Update contact status
        [HttpPut("{id}/status")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] ContactStatus status)
        {
            var updated = await _contactService.UpdateStatusAsync(id, status);
            if (updated == null)
                return NotFound(new { Message = "Contact not found" });

            return Ok(updated);
        }

        // ✅ Delete contact
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var success = await _contactService.DeleteAsync(id);
            if (!success)
                return NotFound(new { Message = "Contact not found" });

            return NoContent();
        }
    }
}
