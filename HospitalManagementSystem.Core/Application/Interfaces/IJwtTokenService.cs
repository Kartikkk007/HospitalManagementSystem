using System.Threading.Tasks;

namespace HospitalManagementSystem.Core.Application.Interfaces;

public interface IJwtTokenService
{
    Task<string?> GenerateTokenAsync(string email, string password);
}
