using DataAccess.Models;
using Services.Models;

namespace Services;

public interface IAccountService
{
    List<Account> GetAllAccounts();

    Account? Login(string email, string password);
    Account? GetAccountById(int accountId);
    (bool Success, string? Error) RegisterStudent(string fullName, string email, string password, string? phone);
    (bool Success, string? Error) UpdateOwnProfile(int accountId, string fullName, string email, string? newPassword,
        string? phone, string? specialization, DateTime? dateOfBirth);

    /// <summary>
    /// Creates an Account. If role = Instructor, also creates the linked Instructors row.
    /// If role = Student, also creates the linked Students row.
    /// </summary>
    (bool Success, string? Error) CreateAccount(string fullName, string email, string? phone, AccountRole role);

    /// <summary>Fails with an error if the target account is an Admin account (Admin accounts cannot be edited).</summary>
    (bool Success, string? Error) UpdateAccount(int accountId, string fullName, string email, string? phone, bool isActive);

    /// <summary>
    /// Deletes the Account (and its linked Instructor/Student row, if any and if that person has no courses/enrollments).
    /// Fails with an error if the target account is an Admin account (Admin accounts cannot be deleted).
    /// </summary>
    (bool Success, string? Error) DeleteAccount(int accountId);
}

