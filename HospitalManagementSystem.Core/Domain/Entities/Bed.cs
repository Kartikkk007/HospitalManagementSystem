using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class Bed
{
    public Guid Id { get; set; } = Guid.NewGuid();   
    public string WardName { get; set; } = string.Empty; // e.g., "ICU", "General Ward B"
    public string BedNumber { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }


    public bool IsOccupied => OccupantId.HasValue;
    public Guid? OccupantId { get; set; } // Foreign key reference to Patient

  
    public Patient? Occupant { get; set; }
}