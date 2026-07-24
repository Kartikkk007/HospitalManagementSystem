using HospitalManagementSystem.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class RosterShift
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } // The employee whose shift it is currtly
    public User? User { get; set; }

    public DateTime Date { get; set; }
    public ShiftType Shift { get; set; }
}