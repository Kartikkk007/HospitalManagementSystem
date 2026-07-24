using HospitalManagementSystem.Core.Application.Interfaces;
using HospitalManagementSystem.Core.Domain.Entities;
using HospitalManagementSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Infrastructure.Services;

public class AdmissionService : IAdmissionService
{
    private readonly HospitalDbContext _context;

    public AdmissionService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bed>> GetVolatileBedMapAsync()
    {
        
        return await _context.Beds.Include(b => b.Occupant).ToListAsync();
    }

    public async Task<bool> AllocateBedAsync(Guid bedId, Guid patientId)
    {
        var bed = await _context.Beds.FindAsync(bedId);
        var patient = await _context.Patients.FindAsync(patientId);

        if (bed == null || patient == null || bed.IsOccupied)
            return false;

        // B logic 
        bed.OccupantId = patientId;
        patient.IsCurrentlyAdmitted = true;
        patient.CurrentAdmissionStatus = Core.Domain.Enums.AdmissionType.IPD;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DischargePatientFromBedAsync(Guid bedId)
    {
        var bed = await _context.Beds.Include(b => b.Occupant).FirstOrDefaultAsync(b => b.Id == bedId);

        if (bed == null || !bed.IsOccupied)
            return false;

        if (bed.Occupant != null)
        {
            bed.Occupant.IsCurrentlyAdmitted = false;
        }

        bed.OccupantId = null;

        await _context.SaveChangesAsync();
        return true;
    }
}