using DataAccess.Models;
using Repositories;
using Services.Models;

namespace Services.Impl;

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

    public List<Account> GetAllAccounts() => _accountRepository.GetAll();
    public Account? GetAccountById(int accountId) => _accountRepository.GetById(accountId);

    public  Account? Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return null;

        var account = _accountRepository.GetByEmail(email.Trim());
        if (account == null || !account.IsActive || account.Password != password) return null;

        return account;
    }

    public  (bool Success, string? Error) CreateAccount(string fullName, string email, string? phone, AccountRole role)
    {
        var error = Validate(fullName, email);
        if (error != null) return (false, error);

        var existing = _accountRepository.GetByEmail(email.Trim());
        if (existing != null)
            return (false, "Email này đã được sử dụng cho một tài khoản khác.");

        var account = new Account
        {
            FullName = fullName.Trim(),
            Email = email.Trim(),
            Password = "123",
            Role = (int)role,
            IsActive = true
        };

        _accountRepository.Add(account);
        if (role == AccountRole.Instructor)
            _instructorRepository.Add(new Instructor { AccountId = account.Id, Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim() });
        else if (role == AccountRole.Student)
            _studentRepository.Add(new Student { AccountId = account.Id, Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim() });
        return (true, null);
    }

    public  (bool Success, string? Error) RegisterStudent(string fullName, string email, string password, string? phone)
    {
        var error = Validate(fullName, email);
        if (error != null) return (false, error);
        if (string.IsNullOrWhiteSpace(password) || password.Length < 3) return (false, "Mật khẩu phải có ít nhất 3 ký tự.");
        if (_accountRepository.GetByEmail(email.Trim()) != null) return (false, "Email này đã được sử dụng.");

        var account = new Account { FullName = fullName.Trim(), Email = email.Trim(), Password = password, Role = (int)AccountRole.Student, IsActive = true, CreatedDate = DateTime.Now };
        _accountRepository.Add(account);
        _studentRepository.Add(new Student { AccountId = account.Id, Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim() });
        return (true, null);
    }

    public (bool Success, string? Error) UpdateOwnProfile(int accountId, string fullName, string email, string? newPassword,
        string? phone, string? specialization, DateTime? dateOfBirth)
    {
        var error = Validate(fullName, email);
        if (error != null) return (false, error);
        if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < 3)
            return (false, "Mật khẩu mới phải có ít nhất 3 ký tự.");

        var account = _accountRepository.GetById(accountId);
        if (account == null) return (false, "Không tìm thấy tài khoản.");
        var existingWithEmail = _accountRepository.GetByEmail(email.Trim());
        if (existingWithEmail != null && existingWithEmail.Id != accountId)
            return (false, "Email này đã được sử dụng cho một tài khoản khác.");

        account.FullName = fullName.Trim();
        account.Email = email.Trim();
        if (!string.IsNullOrWhiteSpace(newPassword)) account.Password = newPassword;
        _accountRepository.Update(account);

        if (account.Instructor != null)
        {
            account.Instructor.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            account.Instructor.Specialization = string.IsNullOrWhiteSpace(specialization) ? null : specialization.Trim();
            _instructorRepository.Update(account.Instructor);
        }
        else if (account.Student != null)
        {
            account.Student.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            account.Student.DateOfBirth = dateOfBirth;
            _studentRepository.Update(account.Student);
        }

        return (true, null);
    }

    public  (bool Success, string? Error) UpdateAccount(int accountId, string fullName, string email, string? phone, bool isActive)
    {
        var error = Validate(fullName, email);
        if (error != null) return (false, error);

        var account = _accountRepository.GetById(accountId);
        if (account == null) return (false, "Không tìm thấy tài khoản.");

        if (account.Role == (int)AccountRole.Admin)
            return (false, "Không được phép sửa thông tin tài khoản Admin.");

        var existingWithEmail = _accountRepository.GetByEmail(email.Trim());
        if (existingWithEmail != null && existingWithEmail.Id != accountId)
            return (false, "Email này đã được sử dụng cho một tài khoản khác.");

        account.FullName = fullName.Trim();
        account.Email = email.Trim();
        account.IsActive = isActive;
        _accountRepository.Update(account);

        if (account.Instructor != null)
        {
            var instructor = _instructorRepository.GetById(account.Instructor.Id);
            if (instructor != null)
            {
                instructor.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
                _instructorRepository.Update(instructor);
            }
        }

        // Keep the linked Student row (enrollment identity) in sync.
        if (account.Student != null)
        {
            var student = _studentRepository.GetById(account.Student.Id);
            if (student != null)
            {
                student.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
                _studentRepository.Update(student);
            }
        }

        return (true, null);
    }

    public  (bool Success, string? Error) DeleteAccount(int accountId)
    {
        var account = _accountRepository.GetById(accountId);
        if (account == null) return (false, "Không tìm thấy tài khoản.");

        if (account.Role == (int)AccountRole.Admin)
            return (false, "Không được phép xóa tài khoản Admin.");

        if (account.Instructor != null)
        {
            var courseCount = _instructorRepository.GetCourseCount(account.Instructor.Id);
            if (courseCount > 0)
                return (false, "Không thể xóa Instructor còn đang phụ trách khóa học. Hãy chuyển/xóa khóa học trước.");

            _instructorRepository.Delete(account.Instructor.Id);
        }

        if (account.Student != null)
        {
            var enrollmentCount = _studentRepository.GetEnrollmentCount(account.Student.Id);
            if (enrollmentCount > 0)
                return (false, "Không thể xóa Student còn đang có lượt đăng ký khóa học. Hãy xử lý đăng ký trước.");

            // No delete method for a lone Student row is exposed on purpose - Students may still
            // be referenced historically. Only the Account (login) is removed in that case.
        }

        _accountRepository.Delete(accountId);
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

