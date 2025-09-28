using HarmanKnowledgeHubPortal.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task<bool> ExistsAsync(string email);
        Task<List<User>> GetAllAsync();
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
