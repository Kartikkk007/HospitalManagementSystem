using HospitalManagementSystem.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Core.Application.Interfaces;

public interface IPatientService {

    Task<Guid> RegisterPatientAsync(Patient patient);
    Task<List<Patient>> SearchPatientsAsync(string searchTerm);
    Task<Patient?> GetPatientByIdAsync(Guid id);

}
