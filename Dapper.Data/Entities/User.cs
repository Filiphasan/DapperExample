using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Core.Data.Interfaces;

namespace Dapper.Data.Entities;

[Table("DP_USER")]
public class User : IDbEntity
{
    [Key] [Column("ID")] public int Id { get; set; }
    [Column("PUBLIC_ID")] public string PublicId { get; set; } = null!;
    [Column("FIRST_NAME")] public string FirstName { get; set; } = null!;
    [Column("LAST_NAME")] public string LastName { get; set; } = null!;
    [Column("FULL_NAME")] public string FullName { get; set; } = null!;
    [Column("USERNAME")] public string Username { get; set; } = null!;
    [Column("PASSWORD")] public string Password { get; set; } = null!;
    [Column("LAST_LOGIN_DATE")] public DateTime? LastLoginDate { get; set; }
    [Column("CREATE_DATE")] public DateTime? CreateDate { get; set; }
    [Column("UPDATE_DATE")] public DateTime? UpdateDate { get; set; }
}