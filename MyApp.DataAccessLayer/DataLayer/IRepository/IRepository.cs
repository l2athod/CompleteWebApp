using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DataAccessLayer.DataLayer.IRepository
{
    public interface IRepository<T> where T : class
    {
        // List of generic methods
        List<T> GetAll(Expression<Func<T, bool>>? expression=null, string? includeItems = null);
        T Get(Expression<Func<T, bool>> expression, string? includeItems = null);
        void Add(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);
    }
}
