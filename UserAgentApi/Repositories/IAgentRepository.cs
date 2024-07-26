using UserAgentApi.Models;

namespace UserAgentApi.Repositories
{
    public interface IAgentRepository : IRepository<Agent>
    {
        Task<IEnumerable<Agent>> GetAllWithUsersAsync();
        Task<Agent> GetByIdWithUsersAsync(int id);
    }
}
