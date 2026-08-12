namespace HospitalManagementSystem.BlazorApp.Models.Api;

public sealed record DashboardSummaryResponse(
    int TotalPatients,
    int ActiveInpatients,
    int TotalBeds,
    int OccupiedBeds,
    int AvailableBeds,
    int OccupancyRate,
    int TodayAppointments,
    int ActiveDoctors);

public sealed record BedStatusResponse(
    Guid Id,
    string WardName,
    string BedNumber,
    decimal DailyRate,
    bool IsOccupied,
    string? OccupantName);

public sealed record TodayAppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    string Specialty,
    DateTime AppointmentDate,
    string Status);

public sealed record DoctorDirectoryResponse(
    Guid Id,
    string FullName,
    string Specialty,
    decimal ConsultationFee,
    string ContactNumber);
