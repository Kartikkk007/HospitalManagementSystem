
using HospitalManagementSystem.Core.Application.Interfaces;
using HospitalManagementSystem.Core.Domain.Entities;
using HospitalManagementSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly HospitalDbContext _context;

    public AppointmentService(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<List<Doctor>> GetActiveDoctorsAsync()
    {
  
        return await _context.Doctors.ToListAsync();
    }

    public async Task<Doctor?> GetDoctorByIdAsync(Guid doctorId)
    {
        return await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
    }

    public async Task<Guid> AddDoctorAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return doctor.Id;
    }

    public async Task<bool> BookAppointmentAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(Guid doctorId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }
}