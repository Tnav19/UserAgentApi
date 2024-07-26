using Microsoft.EntityFrameworkCore;
using UserAgentApi.Data;
using UserAgentApi.Models;

namespace UserAgentApi.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllWithAgentsAsync()
        {
            return await _context.Users.Include(u => u.Agent).ToListAsync();
        }

        public async Task<User> GetByIdWithAgentAsync(int id)
        {
            return await _context.Users.Include(u => u.Agent)
                                       .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
