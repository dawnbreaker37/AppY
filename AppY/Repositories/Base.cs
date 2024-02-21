using AppY.Data;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class Base<T> : IBase<T> where T : class
    {
        private readonly Context _context;
        public Base(Context context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<int> GetCountAsync(T entity)
        {
            if (entity != null) return await _context.Set<T>().CountAsync();
            else return 0;
        }
    }
}
