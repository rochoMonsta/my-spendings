namespace MySpendings.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IOutlayRepository Outlay { get; }
        IUserRepository User { get; }
        Task SaveAsync();
    }
}
