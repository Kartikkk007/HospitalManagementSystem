using HospitalManagementSystem.Core.Application.Interfaces;
using HospitalManagementSystem.Core.Domain.Entities;
using HospitalManagementSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Infrastructure.Services;

public class PatientService : IPatientService
{
    private readonly HospitalDbContext _context;

    public PatientService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> RegisterPatientAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient.Id;
    }

    public async Task<List<Patient>> SearchPatientsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _context.Patients.ToListAsync();
        }

        searchTerm = searchTerm.ToLower();
        return await _context.Patients
            .Where(p => p.FirstName.ToLower().Contains(searchTerm) ||
                        p.LastName.ToLower().Contains(searchTerm) ||
                        p.PhoneNumber.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<Patient?> GetPatientByIdAsync(Guid id)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
    }
}