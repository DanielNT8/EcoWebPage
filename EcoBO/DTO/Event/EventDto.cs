namespace EcoBO.DTO.Event
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public string Status { get; set; } // DRAFT, PUBLISHED...

        // Cờ kiểm tra: User hiện tại đã tham gia chưa? (Để Frontend hiện nút "Đã tham gia")
        public bool IsRegistered { get; set; }
    }
}