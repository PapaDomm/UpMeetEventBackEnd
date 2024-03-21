namespace UpMeetEventBackend.Models.DTOs.UserDTOs
{
    public class PutUserDTO
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? Bio { get; set; }

        public string? Password { get; set; }

        public virtual IFormFile? Image { get; set; }
    }
}
