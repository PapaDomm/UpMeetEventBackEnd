using System;
using System.Collections.Generic;

namespace UpMeetEventBackend.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public string Path { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
