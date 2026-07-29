using HospitalManagementSystem.BlazorApp.Components;
using HospitalManagementSystem.BlazorApp.Components.Account;
using HospitalManagementSystem.Core.Domain.Entities;
using HospitalManagementSystem.Infrastructure.Database;
using HospitalManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.BlazorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddIdentityCookies();


              
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

          
            
            
            //  unified context pointing to the Infrastructure migrations assembly
            builder.Services.AddDbContext<HospitalDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly("HospitalManagementSystem.Infrastructure")
                ));
            builder.Services.AddScoped<HospitalManagementSystem.Core.Application.Interfaces.IPatientService, HospitalManagementSystem.Infrastructure.Services.PatientService>();
            builder.Services.AddScoped<HospitalManagementSystem.Core.Application.Interfaces.IAdmissionService, HospitalManagementSystem.Infrastructure.Services.AdmissionService>();
            builder.Services.AddScoped<HospitalManagementSystem.Core.Application.Interfaces.IAppointmentService, HospitalManagementSystem.Infrastructure.Services.AppointmentService>();
            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
                .AddEntityFrameworkStores<HospitalDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();


            app.MapAdditionalIdentityEndpoints();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

                if (!dbContext.Beds.Any())
                {
                    dbContext.Beds.AddRange(
                        new Bed { WardName = "ICU", BedNumber = "ICU-101", DailyRate = 2500 },
                        new Bed { WardName = "ICU", BedNumber = "ICU-102", DailyRate = 2500 },
                        new Bed { WardName = "General Ward A", BedNumber = "G-201", DailyRate = 800 },
                        new Bed { WardName = "General Ward A", BedNumber = "G-202", DailyRate = 800 },
                        new Bed { WardName = "Pediatrics", BedNumber = "P-301", DailyRate = 1200 }
                    );
                    dbContext.SaveChanges();
                }

                if (!dbContext.Doctors.Any())
                {
                    dbContext.Doctors.AddRange(
                      new Doctor { FirstName = "Aman", LastName = "Verma", Specialty = "Cardiology", ConsultationFee = 500, ContactNumber = "+91 9876543210" },
                      new Doctor { FirstName = "Priya", LastName = "Sharma", Specialty = "Pediatrics", ConsultationFee = 600, ContactNumber = "+91 9876543211" },
                      new Doctor { FirstName = "Rajesh", LastName = "Gupta", Specialty = "Neurology", ConsultationFee = 800, ContactNumber = "+91 9876543212" }
                    );
                    dbContext.SaveChanges();
                }
            } // <-- Scope closes cleanly here!

            // 2. Start the Application
            app.Run();
        }
        }
    }

