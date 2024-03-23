namespace UpMeetEventBackend.Models.DTOs.EventUserDTOs
{
    public class PostEventDTO
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string City { get; set; } = null!;

        public string State { get; set; } = null!;

        public bool Expired { get; set; }

        public virtual IFormFile? Image { get; set; }
    }
}
