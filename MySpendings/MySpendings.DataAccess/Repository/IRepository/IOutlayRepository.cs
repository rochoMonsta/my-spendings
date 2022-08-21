using MySpendings.Models;

namespace MySpendings.DataAccess.Repository.IRepository
{
    public interface IOutlayRepository : IRepository<Outlay>
    {
        void Update(Outlay outlay);
    }
}
