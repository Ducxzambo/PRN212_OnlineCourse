using DataAccess.Models;

namespace Presentation.Helpers;

/// <summary>Holds the currently logged-in Admin Account (parallel to InstructorSession).</summary>
public static class AdminSession
{
    public static Account? Current { get; set; }
}
