namespace QuanLySanBong.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();

        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
    }
}
