using System.Linq.Expressions;

namespace Repositories.Common;

public interface IGenericRepository<T> where T : class
{
    List<T> GetAll();
    T? GetById(int id);
    List<T> Find(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}

