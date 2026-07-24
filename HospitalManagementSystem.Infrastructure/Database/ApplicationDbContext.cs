using HospitalManagementSystem.Core.Domain.Entities;
using HospitalManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Infrastructure.Database;

public class HospitalDbContext : IdentityDbContext<ApplicationUser>
{

    public HospitalDbContext(DbContextOptions options) : base(options)
    {
    }


    public DbSet<Patient> Patients { get; set; } = default!;
    public DbSet<Doctor> Doctors { get; set; } = default!;
    public DbSet<Bed> Beds { get; set; } = default!;
    public DbSet<Appointment> Appointments { get; set; } = default!;
    public DbSet<LabOrder> LabOrders { get; set; } = default!;
    public DbSet<RosterShift> RosterShifts { get; set; } = default!;
    public DbSet<Invoice> Invoices { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invoice>().Property(i => i.ConsultationFees).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>().Property(i => i.RoomCharges).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>().Property(i => i.LabFees).HasPrecision(18, 2);
        modelBuilder.Entity<Bed>().Property(b => b.DailyRate).HasPrecision(18, 2);
        modelBuilder.Entity<Doctor>().Property(d => d.ConsultationFee).HasPrecision(18, 2);
        modelBuilder.Entity<LabOrder>().Property(l => l.Cost).HasPrecision(18, 2);
    }
}