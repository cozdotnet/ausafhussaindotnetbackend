using System;
using System.Collections.Generic;

namespace PerCare.Models;

public partial class Pet
{
    public int Petid { get; set; }

    public int? Userid { get; set; }

    public string? Name { get; set; }

    public string? Medicalname { get; set; }

    public DateTime? Date { get; set; }

    public virtual User? User { get; set; }
}
