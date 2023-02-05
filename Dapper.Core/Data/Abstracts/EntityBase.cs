namespace Dapper.Core.Data.Abstracts;

public abstract class EntityBase : EntityBaseUpdateable
{
    public DateTime? LogCreateDate { get; set; }
}