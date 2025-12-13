using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using HappyTools.DependencyInjection.Contracts;
namespace HappyTools.Shared.Identity
{
    public class CurrentUser : ICurrentUser, IScopedDependency
    {
        public bool IsAuthenticated { get; private set; }
        public Guid? Id { get; private set; }
        public string? UserName { get; private set; }
        public string? Name { get; private set; }
        public string? SurName { get; private set; }
        public string? PhoneNumber { get; private set; }
        public bool PhoneNumberVerified { get; private set; }
        public string? Email { get; private set; }
        public bool EmailVerified { get; private set; }
        public Guid? TenantId { get; private set; }
        public string[] Roles { get; private set; } = Array.Empty<string>();
        private Claim[] Claims { get; set; } = Array.Empty<Claim>();

        public void SetClaims(ClaimsPrincipal principal)
        {
            IsAuthenticated = principal.Identity?.IsAuthenticated ?? false;

            if (!IsAuthenticated) return;

            Claims = principal.Claims.ToArray();

            Id = Claims.FirstOrDefault(c => c.Type == "sub")?.Value is { } idStr && Guid.TryParse(idStr, out var id) ? id : null;
            UserName = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            Name = Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            SurName = Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
            PhoneNumber = Claims.FirstOrDefault(c => c.Type == "phoneNumber")?.Value;
            PhoneNumberVerified = Claims.FirstOrDefault(c => c.Type == "phone_number_verified")?.Value == "true";
            Email = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            EmailVerified = Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true";
            TenantId = Claims.FirstOrDefault(c => c.Type == "tenantid")?.Value is { } tenantStr && Guid.TryParse(tenantStr, out var tenantId) ? tenantId : null;
            Roles = Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
        }
        
        public Claim? FindClaim(string claimType) => Claims.FirstOrDefault(c => c.Type == claimType);
        public Claim[] FindClaims(string claimType) => Claims.Where(c => c.Type == claimType).ToArray();
        public Claim[] GetAllClaims() => Claims;
        public bool IsInRole(string roleName) => Roles.Contains(roleName);
    }

}
