using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBServices.Services
{
    public class GenericService<T> where T : class
    {
        private readonly PMTContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericService(PMTContext pMTContext)
        {
            _context = pMTContext;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task UpdateAsync(T existingEntity, T updatedEntity)
        {
            if (existingEntity == null || updatedEntity == null)
            {
                throw new ArgumentException("Both existing and updated entities are required");
            }

            
            _dbSet.Attach(existingEntity);

           
            _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);

          
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
