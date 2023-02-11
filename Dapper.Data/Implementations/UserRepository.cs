using System.Data;
using Dapper.Data.Entities;
using Dapper.Data.Interfaces;

namespace Dapper.Data.Implementations;

public class UserRepository : DapperRepository<User>, IUserRepository
{
    public UserRepository(IDbConnection connection) : base(connection)
    {
    }
}