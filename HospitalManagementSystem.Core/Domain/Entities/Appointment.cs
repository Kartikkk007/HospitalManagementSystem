using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty; // e.g., "10:30 AM"
    public string Status { get; set; } = "Scheduled"; // e.g., "Scheduled", "Completed", "Cancelled"
}