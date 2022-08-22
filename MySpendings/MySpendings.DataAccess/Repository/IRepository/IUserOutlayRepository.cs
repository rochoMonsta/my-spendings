using MySpendings.Models;

namespace MySpendings.DataAccess.Repository.IRepository
{
    public interface IUserOutlayRepository : IRepository<UserOutlay>
    {
        void Update(UserOutlay userOutlay);
    }
}
