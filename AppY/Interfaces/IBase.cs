namespace AppY.Repositories
{
    public interface IBase<T> where T : class
    {
        public Task<int> GetCountAsync(T entity);
        public Task<List<T>> GetAllAsync();
    }
}
