using HarmanKnowledgeHubPortal.Domain.Entities;
using HarmanKnowledgeHubPortal.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Data
{
        public class UserRepository : IUserRepository
        {
            private readonly AppDbContext _context;

            public UserRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task AddAsync(User user)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            public async Task<bool> ExistsAsync(string email)
            {
                return await _context.Users.AnyAsync(u => u.Email == email);
            }

            public async Task<User?> GetByEmailAsync(string email)
            {
                return await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email);
            }

            public async Task<User?> GetByIdAsync(int id)
            {
                return await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }

            public async Task<List<User>> GetAllAsync()
            {
                // We include Roles so we can display them on the frontend
                return await _context.Users.Include(u => u.Roles).ToListAsync();
            }

            public async Task UpdateAsync(User user)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(int id)
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
        }
}

