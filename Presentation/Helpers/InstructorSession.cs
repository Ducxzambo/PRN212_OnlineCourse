using DataAccess.Models;

namespace Presentation.Helpers;

/// <summary>
/// Holds the currently logged-in instructor profile.
/// </summary>
public static class InstructorSession
{
    public static Instructor? Current { get; set; }
}

