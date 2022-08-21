using MySpendings.DataAccess.Data;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;

namespace MySpendings.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDBContext _context;

        public CategoryRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }
    }
}
