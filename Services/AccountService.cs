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

    public async Task<Account?> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) 
            return null;

        // Try to find account by username (email)
        var account = await _accountRepository.GetByEmailAsync(username.Trim());
        if (account == null || !account.IsActive) 
            return null;

        // Validate password
        if (account.Password != password)
            return null;

        return account;
    }

    public async Task<(bool Success, string? Error)> CreateAccountAsync(string fullName, string email, string? phone, AccountRole role)
    {
        var error = Validate(fullName, email);
        if (error != null) return (false, error);

        var existing = await _accountRepository.GetByEmailAsync(email.Trim());
        if (existing != null)
            return (false, "Email này đã được sử dụng cho một tài khoản khác.");

        var account = new Account
        {
            FullName = fullName.Trim(),
            Email = email.Trim(),
            Password = email.Trim(), // Default password is email
            Role = (int)role,
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        await _accountRepository.AddAsync(account);

        // Reload to get the ID
        account = await _accountRepository.GetByEmailAsync(email.Trim());
        if (account == null)
            return (false, "Không thể tạo tài khoản.");

        // Create linked Instructor or Student record
        if (role == AccountRole.Instructor)
        {
            var instructor = new Instructor
            {
                AccountId = account.Id,
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim()
            };
            await _instructorRepository.AddAsync(instructor);
        }
        else if (role == AccountRole.Student)
        {
            var student = new Student
            {
                AccountId = account.Id,
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim()
            };
            await _studentRepository.AddAsync(student);
        }

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

        // Update related Instructor or Student record
        if (account.Role == (int)AccountRole.Instructor && account.Instructor != null)
        {
            account.Instructor.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            await _instructorRepository.UpdateAsync(account.Instructor);
        }

        if (account.Role == (int)AccountRole.Student && account.Student != null)
        {
            account.Student.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            await _studentRepository.UpdateAsync(account.Student);
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAccountAsync(int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null) return (false, "Không tìm thấy tài khoản.");

        if (account.Role == (int)AccountRole.Admin)
            return (false, "Không được phép xóa tài khoản Admin.");

        // Check if instructor has courses
        if (account.Role == (int)AccountRole.Instructor && account.Instructor != null)
        {
            var courseCount = await _instructorRepository.GetCourseCountAsync(account.Instructor.Id);
            if (courseCount > 0)
                return (false, "Không thể xóa Instructor còn đang phụ trách khóa học. Hãy chuyển/xóa khóa học trước.");

            await _instructorRepository.DeleteAsync(account.Instructor.Id);
        }

        // Check if student has enrollments
        if (account.Role == (int)AccountRole.Student && account.Student != null)
        {
            var enrollmentCount = await _studentRepository.GetEnrollmentCountAsync(account.Student.Id);
            if (enrollmentCount > 0)
                return (false, "Không thể xóa Student còn đang có lượt đăng ký khóa học. Hãy xử lý đăng ký trước.");
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
