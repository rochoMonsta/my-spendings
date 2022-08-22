using MySpendings.DataAccess.Data;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;

namespace MySpendings.DataAccess.Repository
{
    public class UserOutlayRepository : Repository<UserOutlay>, IUserOutlayRepository
    {
        private readonly ApplicationDBContext _context;

        public UserOutlayRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public void Update(UserOutlay userOutlay)
        {
            _context.UserOutlays.Update(userOutlay);
        }
    }
}
