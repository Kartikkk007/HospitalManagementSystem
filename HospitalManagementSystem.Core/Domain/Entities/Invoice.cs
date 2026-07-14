using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    public decimal ConsultationFees { get; set; }
    public decimal RoomCharges { get; set; }
    public decimal LabFees { get; set; }

 
    public decimal TotalAmount => ConsultationFees + RoomCharges + LabFees;

    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}