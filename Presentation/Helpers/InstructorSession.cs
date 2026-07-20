using DataAccess.Models;

namespace Presentation.Helpers;

/// <summary>
/// There is no Account/login table in the schema yet, so the "logged in instructor"
/// is simply whichever Instructor row matched the email typed on the login screen.
/// </summary>
public static class InstructorSession
{
    public static Instructor? Current { get; set; }
}
