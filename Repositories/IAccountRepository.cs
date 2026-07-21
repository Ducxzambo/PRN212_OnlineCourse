using DataAccess.Models;

namespace Repositories;

public interface IAccountRepository
{
    Task<List<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
    Task<Account?> GetByEmailAsync(string email);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task DeleteAsync(int id);
}
