using DataAccess.Models;

namespace Presentation.Helpers;

/// <summary>
/// Holds the currently logged-in Student Account (parallel to InstructorSession and AdminSession).
/// </summary>
public static class StudentSession
{
    public static Student? Current { get; set; }
    public static Account? CurrentAccount { get; set; }
}
