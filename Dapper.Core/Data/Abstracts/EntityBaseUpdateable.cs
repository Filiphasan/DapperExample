namespace Dapper.Core.Data.Abstracts;

public abstract class EntityBaseUpdateable
{
    public DateTime? LogUpdatedDateTime { get; set; }
}