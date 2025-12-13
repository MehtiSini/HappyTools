namespace HappyTools.Domain.Entities.Audit.Abstractions
{
    public interface IMayHaveCreator
    {
        Guid? CreatorId { get; set; }
    }

}
