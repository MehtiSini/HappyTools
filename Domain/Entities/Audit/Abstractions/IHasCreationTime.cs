namespace HappyTools.Domain.Entities.Audit.Abstractions
{
    public interface IHasCreationTime
    {
        DateTime? CreationTime { get; set; }
    }

}
