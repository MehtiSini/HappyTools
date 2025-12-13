using HappyTools.Domain.Entities.Concurrency;

namespace HappyTools.Contract.Dtos.Identity
{
    public class IdentityUserUpdateDto : IdentityUserCreateOrUpdateDtoBase, IHasConcurrencyStamp
    {
        public string Password { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
}
