using HospitalManagementSystem.BlazorApp.Models.Api;
using HospitalManagementSystem.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.BlazorApp.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Doctor,Nurse,Receptionist")]
[Route("api/dashboard")]
public sealed class DashboardApiController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IAdmissionService _admissionService;
    private readonly IAppointmentService _appointmentService;

    public DashboardApiController(
        IPatientService patientService,
        IAdmissionService admissionService,
        IAppointmentService appointmentService)
    {
        _patientService = patientService;
        _admissionService = admissionService;
        _appointmentService = appointmentService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetSummary()
    {
        var patients = await _patientService.SearchPatientsAsync(string.Empty);
        var beds = await _admissionService.GetVolatileBedMapAsync();
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        var doctors = await _appointmentService.GetActiveDoctorsAsync();

        var occupiedBeds = beds.Count(b => b.IsOccupied);
        var occupancyRate = beds.Count == 0
            ? 0
            : (int)Math.Round((double)occupiedBeds / beds.Count * 100);

        return Ok(new DashboardSummaryResponse(
            TotalPatients: patients.Count,
            ActiveInpatients: patients.Count(p => p.IsCurrentlyAdmitted),
            TotalBeds: beds.Count,
            OccupiedBeds: occupiedBeds,
            AvailableBeds: beds.Count - occupiedBeds,
            OccupancyRate: occupancyRate,
            TodayAppointments: appointments.Count(IsToday),
            ActiveDoctors: doctors.Count));
    }

    [HttpGet("beds")]
    public async Task<ActionResult<IReadOnlyList<BedStatusResponse>>> GetBeds()
    {
        var beds = await _admissionService.GetVolatileBedMapAsync();

        var response = beds
            .OrderBy(b => b.WardName)
            .ThenBy(b => b.BedNumber)
            .Select(b => new BedStatusResponse(
                Id: b.Id,
                WardName: b.WardName,
                BedNumber: b.BedNumber,
                DailyRate: b.DailyRate,
                IsOccupied: b.IsOccupied,
                OccupantName: b.Occupant == null ? null : $"{b.Occupant.FirstName} {b.Occupant.LastName}"))
            .ToList();

        return Ok(response);
    }

    [HttpGet("appointments/today")]
    public async Task<ActionResult<IReadOnlyList<TodayAppointmentResponse>>> GetTodayAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();

        var response = appointments
            .Where(IsToday)
            .OrderBy(a => a.AppointmentDate)
            .Select(a => new TodayAppointmentResponse(
                Id: a.Id,
                PatientId: a.PatientId,
                PatientName: a.Patient == null ? "Unknown patient" : $"{a.Patient.FirstName} {a.Patient.LastName}",
                DoctorId: a.DoctorId,
                DoctorName: a.Doctor == null ? "Unknown doctor" : $"Dr. {a.Doctor.FirstName} {a.Doctor.LastName}",
                Specialty: a.Doctor?.Specialty ?? "Unassigned",
                AppointmentDate: a.AppointmentDate,
                Status: a.Status))
            .ToList();

        return Ok(response);
    }

    [HttpGet("doctors")]
    public async Task<ActionResult<IReadOnlyList<DoctorDirectoryResponse>>> GetDoctors()
    {
        var doctors = await _appointmentService.GetActiveDoctorsAsync();

        var response = doctors
            .OrderBy(d => d.Specialty)
            .ThenBy(d => d.LastName)
            .Select(d => new DoctorDirectoryResponse(
                Id: d.Id,
                FullName: $"Dr. {d.FirstName} {d.LastName}",
                Specialty: d.Specialty,
                ConsultationFee: d.ConsultationFee,
                ContactNumber: d.ContactNumber))
            .ToList();

        return Ok(response);
    }

    private static bool IsToday(Core.Domain.Entities.Appointment appointment)
    {
        return appointment.AppointmentDate.Date == DateTime.Today;
    }
}
