using HarmanKnowledgeHubPortal.Domain.DTO;
using HarmanKnowledgeHubPortal.Domain.Entities;
using HarmanKnowledgeHubPortal.Domain.Repositories;
using HarmanKnowledgeHubPortal.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IRoleRepository roleRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepo.ExistsAsync(dto.Email))
                throw new Exception("Email already registered");

            var roles = new List<Role>();
            foreach (var roleName in dto.Roles)
            {
                var role = await _roleRepo.GetByNameAsync(roleName);
                if (role == null)
                    throw new Exception($"Role '{roleName}' not found");
                roles.Add(role);
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                // Hash the password before saving
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DateTimeCreated = DateTime.UtcNow,
                Roles = roles
            };

            await _userRepo.AddAsync(user);

            // Return a DTO with a real JWT token
            return new AuthResponseDto
            {
                Email = user.Email,
                Name = user.Name,
                Roles = user.Roles.Select(r => r.Name).ToList(),
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);

            // Verify the hashed password
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Invalid credentials");

            // Return a DTO with a real JWT token
            return new AuthResponseDto
            {
                Email = user.Email,
                Name = user.Name,
                Roles = user.Roles.Select(r => r.Name).ToList(),
                Token = GenerateJwtToken(user)
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name), // This claim is read by User.Identity.Name
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}