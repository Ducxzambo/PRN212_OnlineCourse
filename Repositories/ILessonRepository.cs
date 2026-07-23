using DataAccess.Models;

namespace Repositories;

public interface ILessonRepository
{
    List<Lesson> GetByCourse(int courseId);
    Lesson? GetById(int id);
    int GetNextOrderIndex(int courseId);
    void Add(Lesson lesson);
    void Update(Lesson lesson);
    void Delete(int id);
    void SwapOrderIndex(int lessonId1, int orderIndex1, int lessonId2, int orderIndex2);
}

