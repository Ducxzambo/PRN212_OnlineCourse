using DataAccess.Models;

namespace Repositories;

public interface IAccountRepository
{
    List<Account> GetAll();
    Account? GetById(int id);
    Account? GetByEmail(string email);
    void Add(Account account);
    void Update(Account account);
    void Delete(int id);
}

