using DataAccess.Models;
using Repositories;
using Services.Models;

namespace Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IStudentRepository _studentRepository;

    public AccountService(
        IAccountRepository accountRepository,
        IInstructorRepository instructorRepository,
        IStudentRepository studentRepository)
    {
        _accountRepository = accountRepository;
        _instructorRepository = instructorRepository;
        _studentRepository = studentRepository;
    }

    public Task<List<Account>> GetAllAccountsAsync() => _accountRepository.GetAllAsync();

    public async Task<Account?> LoginAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        var account = await _accountRepository.GetByEmailAsync(email.Trim());
        if (account == null || !account.IsActive) return null;

        return account;
    }

    public async Task<(bool Success, string? Error)> CreateAccountAsync(string fullName, string email, string? phone, AccountRole role)
    {
        var error = Validate(fullName, email);
        if (error != null) return (false, error);

        var existing = await _accountRepository.GetByEmailAsync(email.Trim());
        if (existing != null)
            return (false, "Email này đã được sử dụng cho một tài khoản khác.");

        int? instructorId = null;
        int? studentId = null;

        if (role == AccountRole.Instructor)
        {
            instructorId = await _instructorRepository.AddAsync(new Instructor
            {
                FullName = fullName.Trim(),
                Email = email.Trim(),
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim()
            });
        }
        else if (role == AccountRole.Student)
        {
            studentId = await _studentRepository.AddAsync(new Student
            {
                FullName = fullName.Trim(),
                Email = email.Trim(),
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim()
            });
        }

        var account = new Account
        {
            FullName = fullName.Trim(),
            Email = email.Trim(),
            Role = (int)role,
            IsActive = true,
            InstructorId = instructorId,
            StudentId = studentId
        };

        await _accountRepository.AddAsync(account);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAccountAsync(int accountId, string fullName, string email, string? phone, bool isActive)
    {
        var error = Validate(fullName, email);
        if (error != null) return (false, error);

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null) return (false, "Không tìm thấy tài khoản.");

        if (account.Role == (int)AccountRole.Admin)
            return (false, "Không được phép sửa thông tin tài khoản Admin.");

        var existingWithEmail = await _accountRepository.GetByEmailAsync(email.Trim());
        if (existingWithEmail != null && existingWithEmail.Id != accountId)
            return (false, "Email này đã được sử dụng cho một tài khoản khác.");

        account.FullName = fullName.Trim();
        account.Email = email.Trim();
        account.IsActive = isActive;
        await _accountRepository.UpdateAsync(account);

        // Keep the linked Instructor row (login identity used across Courses) in sync.
        if (account.InstructorId.HasValue)
        {
            var instructor = await _instructorRepository.GetByIdAsync(account.InstructorId.Value);
            if (instructor != null)
            {
                instructor.FullName = fullName.Trim();
                instructor.Email = email.Trim();
                instructor.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
                await _instructorRepository.UpdateAsync(instructor);
            }
        }

        // Keep the linked Student row (enrollment identity) in sync.
        if (account.StudentId.HasValue)
        {
            var student = await _studentRepository.GetByIdAsync(account.StudentId.Value);
            if (student != null)
            {
                student.FullName = fullName.Trim();
                student.Email = email.Trim();
                student.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
                await _studentRepository.UpdateAsync(student);
            }
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAccountAsync(int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null) return (false, "Không tìm thấy tài khoản.");

        if (account.Role == (int)AccountRole.Admin)
            return (false, "Không được phép xóa tài khoản Admin.");

        if (account.InstructorId.HasValue)
        {
            var courseCount = await _instructorRepository.GetCourseCountAsync(account.InstructorId.Value);
            if (courseCount > 0)
                return (false, "Không thể xóa Instructor còn đang phụ trách khóa học. Hãy chuyển/xóa khóa học trước.");

            await _instructorRepository.DeleteAsync(account.InstructorId.Value);
        }

        if (account.StudentId.HasValue)
        {
            var enrollmentCount = await _studentRepository.GetEnrollmentCountAsync(account.StudentId.Value);
            if (enrollmentCount > 0)
                return (false, "Không thể xóa Student còn đang có lượt đăng ký khóa học. Hãy xử lý đăng ký trước.");

            // No delete method for a lone Student row is exposed on purpose - Students may still
            // be referenced historically. Only the Account (login) is removed in that case.
        }

        await _accountRepository.DeleteAsync(accountId);
        return (true, null);
    }

    private static string? Validate(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "Họ tên không được để trống.";
        if (fullName.Length > 150)
            return "Họ tên không được vượt quá 150 ký tự.";
        if (string.IsNullOrWhiteSpace(email))
            return "Email không được để trống.";
        if (!email.Contains('@'))
            return "Email không hợp lệ.";
        if (email.Length > 150)
            return "Email không được vượt quá 150 ký tự.";

        return null;
    }
}
