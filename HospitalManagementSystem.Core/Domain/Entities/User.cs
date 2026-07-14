using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // Managed securely by IdentityService later
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}