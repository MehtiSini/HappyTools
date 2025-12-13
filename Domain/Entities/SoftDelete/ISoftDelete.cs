namespace HappyTools.Domain.Entities.SoftDelete
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }

}
