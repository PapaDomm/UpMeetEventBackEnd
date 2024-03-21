namespace UpMeetEventBackend.Models.DTOs.EventUserDTOs
{
    public class PutEventDTO
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? Expired { get; set; }

        public virtual IFormFile? Image { get; set; }
    }
}
