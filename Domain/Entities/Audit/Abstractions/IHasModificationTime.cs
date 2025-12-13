namespace HappyTools.Domain.Entities.Audit.Abstractions
{
    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; set; }
    }

}
