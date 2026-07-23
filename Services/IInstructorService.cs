using DataAccess.Models;

namespace Services;

public interface IInstructorService
{
    /// <summary>Looks the instructor up by email. Returns null if no match (no password in this schema).</summary>
    Instructor? Login(string email);
}

