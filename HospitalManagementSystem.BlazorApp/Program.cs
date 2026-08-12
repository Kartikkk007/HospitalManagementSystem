using HospitalManagementSystem.BlazorApp.Components;
using HospitalManagementSystem.BlazorApp.Components.Account;
using HospitalManagementSystem.BlazorApp.Services;
using HospitalManagementSystem.Core.Domain.Entities;
using HospitalManagementSystem.Infrastructure.Database;
using HospitalManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using System.Text;

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

            builder.Services.AddControllers();
            builder.Services.AddMudServices();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<CustomJwtAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomJwtAuthenticationStateProvider>());

            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Configuration 'Jwt:Key' is missing.");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AegisCareHMS";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "AegisCareHMSClients";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });


              
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
            builder.Services.AddScoped<HospitalManagementSystem.Core.Application.Interfaces.IJwtTokenService, HospitalManagementSystem.Infrastructure.Services.JwtTokenService>();
            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
                .AddRoles<IdentityRole>()
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers();
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

                // Seed Roles and Users
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles = { "Admin", "Doctor", "Nurse", "Receptionist" };
                foreach (var role in roles)
                {
                    if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                    }
                }

                var seedUsers = new[]
                {
                    new { Email = "admin@hospital.com", Role = "Admin" },
                    new { Email = "doctor@hospital.com", Role = "Doctor" },
                    new { Email = "nurse@hospital.com", Role = "Nurse" },
                    new { Email = "staff@hospital.com", Role = "Receptionist" }
                };

                foreach (var seedUser in seedUsers)
                {
                    var user = userManager.FindByEmailAsync(seedUser.Email).GetAwaiter().GetResult();
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = seedUser.Email,
                            Email = seedUser.Email,
                            EmailConfirmed = true
                        };
                        var result = userManager.CreateAsync(user, "P@ssword123!").GetAwaiter().GetResult();
                        if (result.Succeeded)
                        {
                            userManager.AddToRoleAsync(user, seedUser.Role).GetAwaiter().GetResult();
                        }
                    }
                }
            }

            
            app.Run();
        }
        }
    }

