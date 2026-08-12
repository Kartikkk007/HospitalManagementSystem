
using HospitalManagementSystem.Core.Domain.Entities;

namespace HospitalManagementSystem.Core.Application.Interfaces;

public interface IAppointmentService
{
    Task<List<Doctor>> GetActiveDoctorsAsync();
    Task<Doctor?> GetDoctorByIdAsync(Guid doctorId);
    Task<Guid> AddDoctorAsync(Doctor doctor);
    Task<bool> BookAppointmentAsync(Appointment appointment);
    Task<List<Appointment>> GetAppointmentsByDoctorAsync(Guid doctorId);
    Task<List<Appointment>> GetAllAppointmentsAsync();
    Task<List<Appointment>> GetTodayAppointmentsAsync(Guid? doctorId = null);
    Task<bool> CompleteAppointmentAsync(Guid appointmentId, string symptoms, string diagnosis, string prescription);
    Task<List<Appointment>> GetAppointmentsByPatientAsync(Guid patientId);
}