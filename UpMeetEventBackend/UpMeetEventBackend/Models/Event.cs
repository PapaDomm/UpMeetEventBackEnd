using System;
using System.Collections.Generic;

namespace UpMeetEventBackend.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? ImageId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool Expired { get; set; }

    public bool Active { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
