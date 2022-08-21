using MySpendings.DataAccess.Data;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;

namespace MySpendings.DataAccess.Repository
{
    public class OutlayRepository : Repository<Outlay>, IOutlayRepository
    {
        private readonly ApplicationDBContext _context;

        public OutlayRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Outlay outlay)
        {
            _context.Outlays.Update(outlay);
        }
    }
}
