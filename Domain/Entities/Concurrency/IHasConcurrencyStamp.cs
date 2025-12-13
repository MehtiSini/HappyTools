namespace HappyTools.Domain.Entities.Concurrency
{
    public interface IHasConcurrencyStamp
    {
        string? ConcurrencyStamp { get; set; } 
    }
}
