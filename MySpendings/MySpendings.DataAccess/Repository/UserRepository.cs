using MySpendings.DataAccess.Data;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;

namespace MySpendings.DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
    }
}
