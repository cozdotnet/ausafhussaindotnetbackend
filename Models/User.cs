using System;
using System.Collections.Generic;

namespace PerCare.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
