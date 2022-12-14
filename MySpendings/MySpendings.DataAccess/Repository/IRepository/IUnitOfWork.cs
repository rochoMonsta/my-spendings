namespace MySpendings.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IOutlayRepository Outlay { get; }
        IUserRepository User { get; }
        IUserCategoryRepository UserCategory { get; }
        IUserOutlayRepository UserOutlay { get; }
        Task SaveAsync();
    }
}
