using HarmanKnowledgeHubPortal.Domain.DTO;
using HarmanKnowledgeHubPortal.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization; // Uncomment this when JWT is fully implemented

namespace HarmanKnowledgeHubPortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")] // IMPORTANT: Protect this endpoint!
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();

            // Map the User entities to UserDto objects
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Roles = u.Roles.Select(r => r.Name).ToList()
            }).ToList();

            return Ok(userDtos);
        }
    }
}