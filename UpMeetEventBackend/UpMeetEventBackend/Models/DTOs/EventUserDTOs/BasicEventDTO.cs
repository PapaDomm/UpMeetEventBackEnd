namespace UpMeetEventBackend.Models.DTOs.EventUserDTOs
{
    public class BasicEventDTO
    {
        public int EventId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int? ImageId { get; set; }
        public bool Expired { get; set; }

        public bool Active { get; set; }

        public virtual ImageDTO? Image { get; set; }
    }
}
