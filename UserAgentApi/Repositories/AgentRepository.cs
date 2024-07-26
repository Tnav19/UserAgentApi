using UserAgentApi.Data;
using Microsoft.EntityFrameworkCore;

using UserAgentApi.Models;

namespace UserAgentApi.Repositories
{
    public class AgentRepository : Repository<Agent>, IAgentRepository
    {
        private readonly AppDbContext _context;

        public AgentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Agent>> GetAllWithUsersAsync()
        {
            return await _context.Agents
                                 .Include(a => a.Users) // Include Users
                                 .ToListAsync();
        }

        public async Task<Agent> GetByIdWithUsersAsync(int id)
        {
            return await _context.Agents
                                 .Include(a => a.Users) // Include Users
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
