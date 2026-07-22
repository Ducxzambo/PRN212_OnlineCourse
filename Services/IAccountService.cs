using DataAccess.Models;
using Services.Models;

namespace Services;

public interface IAccountService
{
    Task<List<Account>> GetAllAccountsAsync();

    /// <summary>Looks up an active Account by username/email and validates the password. Returns null if not found, inactive, or password is incorrect.</summary>
    Task<Account?> LoginAsync(string username, string password);

    /// <summary>
    /// Creates an Account. If role = Instructor, also creates the linked Instructors row.
    /// If role = Student, also creates the linked Students row.
    /// </summary>
    Task<(bool Success, string? Error)> CreateAccountAsync(string fullName, string email, string? phone, AccountRole role);

    /// <summary>Fails with an error if the target account is an Admin account (Admin accounts cannot be edited).</summary>
    Task<(bool Success, string? Error)> UpdateAccountAsync(int accountId, string fullName, string email, string? phone, bool isActive);

    /// <summary>
    /// Deletes the Account (and its linked Instructor/Student row, if any and if that person has no courses/enrollments).
    /// Fails with an error if the target account is an Admin account (Admin accounts cannot be deleted).
    /// </summary>
    Task<(bool Success, string? Error)> DeleteAccountAsync(int accountId);
}
