using MySpendings.Models;

namespace MySpendings.DataAccess.Repository.IRepository
{
    public interface IUserCategoryRepository : IRepository<UserCategory>
    {
        void Update(UserCategory userCategory);
    }
}
