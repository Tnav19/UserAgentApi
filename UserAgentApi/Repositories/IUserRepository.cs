using UserAgentApi.Models;

namespace UserAgentApi.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllWithAgentsAsync();

        Task<User> GetByIdWithAgentAsync(int id);
    }
}
