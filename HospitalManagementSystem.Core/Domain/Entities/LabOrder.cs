using HospitalManagementSystem.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class LabOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    public string TestName { get; set; } = string.Empty; // e.g., "Complete Blood Count"
    public TestCategory Category { get; set; }
    public string ResultNotes { get; set; } = string.Empty;
    public decimal Cost { get; set; }

    public string Status { get; set; } = "Pending";
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
}