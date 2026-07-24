using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class Bed
{
    public Guid Id { get; set; } = Guid.NewGuid();   
    public string WardName { get; set; } = string.Empty; // like ICU or General Ward
    public string BedNumber { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }


    public bool IsOccupied => OccupantId.HasValue;
    public Guid? OccupantId { get; set; } 

  
    public Patient? Occupant { get; set; }
}