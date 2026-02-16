using Microsoft.EntityFrameworkCore;
using TaskManagementAssesmentt.Repositories.Data;
using TaskManagementAssesmentt.Repositories.IRepository;

namespace TaskManagementAssesmentt.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TaskDbContext Context;

        public Repository(TaskDbContext context)
        {
            Context = context;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            Context.Set<T>().Update(entity);
            await SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            Context.Set<T>().Remove(entity);
            await SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
