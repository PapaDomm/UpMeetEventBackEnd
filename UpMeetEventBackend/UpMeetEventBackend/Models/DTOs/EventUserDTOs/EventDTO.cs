using UpMeetEventBackend.Models.DTOs.UserDTOs;

namespace UpMeetEventBackend.Models.DTOs.EventUserDTOs
{
    public class EventDTO
    {
        public int EventId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int? ImageId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string City { get; set; } = null!;

        public string State { get; set; } = null!;

        public bool Expired { get; set; }

        public bool Active { get; set; }

        public virtual ImageDTO? Image { get; set; }

        public virtual ICollection<BasicUserDTO> Users { get; set; } = new List<BasicUserDTO>();
    }
}
