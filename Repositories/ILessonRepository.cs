using DataAccess.Models;

namespace Repositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetByCourseAsync(int courseId);
    Task<Lesson?> GetByIdAsync(int id);
    Task<int> GetNextOrderIndexAsync(int courseId);
    Task AddAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task DeleteAsync(int id);
    Task SwapOrderIndexAsync(int lessonId1, int orderIndex1, int lessonId2, int orderIndex2);
}
