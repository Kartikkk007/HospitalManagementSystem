using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;

namespace HospitalManagementSystem.BlazorApp.Services;

public class CustomJwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IConfiguration _configuration;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomJwtAuthenticationStateProvider(IJSRuntime jsRuntime, IConfiguration configuration)
    {
        _jsRuntime = jsRuntime;
        _configuration = configuration;
    }

    private TokenValidationParameters GetValidationParameters()
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Configuration 'Jwt:Key' is missing.");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "AegisCareHMS";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "AegisCareHMSClients";

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(_anonymous);
            }

            var handler = new JwtSecurityTokenHandler();
            var validationParams = GetValidationParameters();

            var principal = handler.ValidateToken(token, validationParams, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                return new AuthenticationState(_anonymous);
            }

            var identity = new ClaimsIdentity(principal.Claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (SecurityTokenException)
        {
            // Token signature or lifetime validation failed (e.g. tampered or expired)
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
            catch { }
            return new AuthenticationState(_anonymous);
        }
        catch (InvalidOperationException)
        {
            // Prerendering or JS not available yet
            return new AuthenticationState(_anonymous);
        }
        catch (Exception)
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParams = GetValidationParameters();
        var principal = handler.ValidateToken(token, validationParams, out _);

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);

        var identity = new ClaimsIdentity(principal.Claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }
        catch { }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
