using System;
using System.Collections.Generic;
using System.Text;
using HospitalManagementSystem.Core.Domain.Entities; 
using System.Threading.Tasks;

namespace HospitalManagementSystem.Core.Application.Interfaces;

public interface IAdmissionService
{
    Task<List<Bed>> GetVolatileBedMapAsync();
    Task<bool> AllocateBedAsync(Guid bedId, Guid patientId);
    Task<bool> DischargePatientFromBedAsync(Guid bedId);

}
