using DataAccess.Models;

namespace Repositories;

public interface ICategoryRepository
{
    List<Category> GetAll();
    Category? GetById(int id);
    bool ExistsByName(string name, int? excludeId = null);
    int GetCourseCount(int categoryId);
    void Add(Category category);
    void Update(Category category);
    void Delete(int id);
}

