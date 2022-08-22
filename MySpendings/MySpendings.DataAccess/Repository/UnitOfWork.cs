using MySpendings.DataAccess.Data;
using MySpendings.DataAccess.Repository.IRepository;

namespace MySpendings.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Outlay = new OutlayRepository(_context);
            User = new UserRepository(_context);
            UserCategory = new UserCategoryRepository(_context);
            UserOutlay = new UserOutlayRepository(_context);
        }

        public ICategoryRepository Category { get; private set; }

        public IOutlayRepository Outlay { get; private set; }

        public IUserRepository User { get; private set; }

        public IUserCategoryRepository UserCategory { get; private set; }

        public IUserOutlayRepository UserOutlay { get; private set; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
