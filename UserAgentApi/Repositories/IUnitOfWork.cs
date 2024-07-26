using UserAgentApi.Models;

namespace UserAgentApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IAgentRepository Agents { get; }
        Task<int> CompleteAsync();
    }
}
