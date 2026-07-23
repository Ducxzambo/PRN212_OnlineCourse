using DataAccess.Models;
using Repositories;

namespace Services.Impl;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;

    public InstructorService(IInstructorRepository instructorRepository)
    {
        _instructorRepository = instructorRepository;
    }

    public  Instructor? Login(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return _instructorRepository.GetByEmail(email.Trim());
    }
}

