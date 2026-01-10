using System.ComponentModel.DataAnnotations;

namespace EcoBO.DTO.Event
{
    public class CreateEventRequest
    {
        [Required(ErrorMessage = "Tên sự kiện là bắt buộc")]
        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? MaxParticipants { get; set; } // Null = Không giới hạn
    }
}