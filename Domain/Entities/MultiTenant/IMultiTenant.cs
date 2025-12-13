namespace HappyTools.Domain.Entities.MultiTenant
{
    public interface IMultiTenant
    {
        //
        // Summary:
        //     Id of the related tenant.
        Guid? TenantId { get; set; }
    }

}
