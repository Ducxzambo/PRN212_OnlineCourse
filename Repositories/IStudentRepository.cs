using DataAccess.Models;

namespace Repositories;

public interface IStudentRepository
{
    Student? GetById(int id);
    Student? GetByEmail(string email);
    int Add(Student student);
    void Update(Student student);
    int GetEnrollmentCount(int studentId);
}

