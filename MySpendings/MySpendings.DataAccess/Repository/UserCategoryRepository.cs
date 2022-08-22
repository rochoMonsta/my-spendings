using MySpendings.DataAccess.Data;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;

namespace MySpendings.DataAccess.Repository
{
    public class UserCategoryRepository : Repository<UserCategory>, IUserCategoryRepository
    {
        private readonly ApplicationDBContext _context;

        public UserCategoryRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public void Update(UserCategory userCategory)
        {
            _context.UserCategories.Update(userCategory);
        }
    }
}
