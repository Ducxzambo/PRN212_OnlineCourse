using DataAccess.Models;

namespace Repositories;

public interface IInstructorRepository
{
    Task<Instructor?> GetByEmailAsync(string email);
    Task<Instructor?> GetByIdAsync(int id);
    Task<List<Instructor>> GetAllAsync();
}
