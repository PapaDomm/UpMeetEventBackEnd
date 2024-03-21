namespace UpMeetEventBackend.Models.DTOs.UserDTOs
{
    public class BasicUserDTO
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? Bio { get; set; }

        public virtual ImageDTO? Image { get; set; }
    }
}
