using DataAccess.Models;
using Repositories;

namespace Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;

    public InstructorService(IInstructorRepository instructorRepository)
    {
        _instructorRepository = instructorRepository;
    }

    public async Task<Instructor?> LoginAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await _instructorRepository.GetByEmailAsync(email.Trim());
    }
}
