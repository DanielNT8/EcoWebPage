using System.ComponentModel.DataAnnotations;

namespace EcoBO.DTO.Event
{
    public class JoinEventRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string ParticipantName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string ParticipantEmail { get; set; }

        public string ParticipantPhone { get; set; }
    }
}