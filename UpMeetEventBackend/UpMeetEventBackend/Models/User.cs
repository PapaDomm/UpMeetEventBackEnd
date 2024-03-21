using System;
using System.Collections.Generic;

namespace UpMeetEventBackend.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? Bio { get; set; }

    public int? ImageId { get; set; }

    public bool Active { get; set; }

    public string? Password { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
