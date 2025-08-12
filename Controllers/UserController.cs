using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteFlixAPI.Data;
using NoteFlixAPI.DTOs;
using NoteFlixAPI.Models;
using System.Security.Claims;

namespace NoteFlixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("premium-status")]
        public async Task<ActionResult<PremiumStatusResponse>> GetPremiumStatus()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.IsPremium })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { error = "Kullanıcı bulunamadı" });
            }

            return Ok(new PremiumStatusResponse
            {
                IsPremium = user.IsPremium,
                PremiumExpiresAt = null // Şimdilik süresiz premium
            });
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserInfo>> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserInfo
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    IsPremium = u.IsPremium
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { error = "Kullanıcı bulunamadı" });
            }

            return Ok(user);
        }
    }
}
