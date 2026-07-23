using DataAccess.Models;

namespace Repositories;

public interface IInstructorRepository
{
    Instructor? GetByEmail(string email);
    Instructor? GetById(int id);
    List<Instructor> GetAll();
    int GetCourseCount(int instructorId);
    int Add(Instructor instructor);
    void Update(Instructor instructor);
    void Delete(int id);
}

