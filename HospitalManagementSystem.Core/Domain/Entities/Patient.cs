using HospitalManagementSystem.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

    public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string MedicalHistorySummary { get; set; } = string.Empty;


    public AdmissionType CurrentAdmissionStatus { get; set; }
    public bool IsCurrentlyAdmitted { get; set; }
}