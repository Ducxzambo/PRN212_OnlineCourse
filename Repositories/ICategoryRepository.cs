using DataAccess.Models;

namespace Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
}
