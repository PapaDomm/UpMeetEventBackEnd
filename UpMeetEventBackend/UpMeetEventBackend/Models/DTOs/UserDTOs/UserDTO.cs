using UpMeetEventBackend.Models.DTOs.EventUserDTOs;

namespace UpMeetEventBackend.Models.DTOs.UserDTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? Bio { get; set; }

        public int? ImageId { get; set; }

        public bool Active { get; set; }

        public virtual ImageDTO? Image { get; set; }

        public virtual ICollection<BasicEventDTO> Events { get; set; } = new List<BasicEventDTO>();
    }
}
