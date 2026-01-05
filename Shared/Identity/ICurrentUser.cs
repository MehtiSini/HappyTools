using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Shared.Identity
{
    public interface ICurrentUser 
    {
        bool IsAuthenticated { get; }

        Guid? Id { get; }

        string? UserName { get; }

        string? Name { get; }

        string? SurName { get; }

        string? PhoneNumber { get; }

        bool PhoneNumberVerified { get; }

        string? Email { get; }

        bool EmailVerified { get; }

        Guid? TenantId { get; }

        string[] Roles { get; }

        void SetClaims(ClaimsPrincipal principal);

        public Claim? FindClaim(string claimType);
        public Claim[] FindClaims(string claimType);
        public Claim[] GetAllClaims();
        public bool IsInRole(string roleName);
    }
}
