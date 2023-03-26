using Microsoft.EntityFrameworkCore;
using MyApp.DataAccessLayer.Data;
using MyApp.DataAccessLayer.DataLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }

        public List<T> GetAll(string? IncludeItems = null)
        {
            IQueryable<T> query = _dbSet;
            if (IncludeItems is not null)
            {
                foreach (var item in IncludeItems.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.ToList();
            //return _dbSet.ToList();
        }

        public T Get(Expression<Func<T, bool>> expression, string? IncludeItems = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(expression);
            if (IncludeItems is not null)
            {
                foreach (var item in IncludeItems.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.FirstOrDefault();
        }
    }
}
