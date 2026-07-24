using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty; 
    public decimal ConsultationFee { get; set; }
    public string ContactNumber { get; set; } = string.Empty;


    public Guid? AssociatedUserId { get; set; }
}