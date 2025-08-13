using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteFlixAPI.Data;
using NoteFlixAPI.DTOs;
using NoteFlixAPI.Models;
using NoteFlixAPI.Services;
using System.Security.Claims;

namespace NoteFlixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            // Email kontrolü
            //if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            //{
            //    return BadRequest(new { error = "Bu email adresi zaten kayıtlı" });
            //}

            // Şifre hash'leme
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Kullanıcı oluşturma
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Name = request.Name,
                IsPremium = true
            };

            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            // Token oluşturma
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Refresh token'ı kaydet
            //var userSession = new UserSession
            //{
            //    UserId = user.Id,
            //    RefreshToken = refreshToken,
            //    ExpiresAt = DateTime.UtcNow.AddDays(7)
            //};

            //_context.UserSessions.Add(userSession);
            //await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    IsPremium = user.IsPremium
                }
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            // Kullanıcı kontrolü
            var user = new User()
            {
                Id = 1,
                Email = "kerem.korkmaz@gmail.com",
                Name = "Kerem Korkmaz",
                IsPremium = true
            };
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            //if (user == null)
            //{
            //    return BadRequest(new { error = "Geçersiz email veya şifre" });
            //}

            // Şifre kontrolü
            //if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            //{
            //    return BadRequest(new { error = "Geçersiz email veya şifre" });
            //}

            // Son giriş zamanını güncelle
            //user.LastLoginAt = DateTime.UtcNow;
            //await _context.SaveChangesAsync();

            // Token oluşturma
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Eski session'ları temizle
            //var oldSessions = await _context.UserSessions
            //    .Where(s => s.UserId == user.Id && s.ExpiresAt < DateTime.UtcNow)
            //    .ToListAsync();
            //_context.UserSessions.RemoveRange(oldSessions);

            // Yeni refresh token'ı kaydet
            var userSession = new UserSession
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            //_context.UserSessions.Add(userSession);
            //await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    IsPremium = user.IsPremium
                }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
        {
            // Refresh token kontrolü
            //var userSession = await _context.UserSessions
            //    .Include(s => s.User)
            //    .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken && s.ExpiresAt > DateTime.UtcNow);

            //if (userSession == null)
            //{
            //    return BadRequest(new { error = "Geçersiz refresh token" });
            //}

            // Yeni token'lar oluştur
            //var accessToken = _jwtService.GenerateAccessToken(userSession.User);
            var accessToken = _jwtService.GenerateAccessToken(new User() { Email = "kerem.korkmaz@gmail.com", Name = "Kerem Korkmaz", Id = 1, IsPremium = true});
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Eski refresh token'ı sil
            //_context.UserSessions.Remove(userSession);

            // Yeni refresh token'ı kaydet
            var newUserSession = new UserSession
            {
                //UserId = userSession.UserId,
                UserId = 1,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            //_context.UserSessions.Add(newUserSession);
            //await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                User = new UserInfo
                {
                    //Id = userSession.User.Id,
                    //Email = userSession.User.Email,
                    //Name = userSession.User.Name,
                    //IsPremium = userSession.User.IsPremium
                    Id = 1,
                    Email = "kerem.korkmaz@gmail.com",
                    Name = "Kerem Korkmaz",
                    IsPremium = true
                }
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Kullanıcının tüm session'larını sil
            var userSessions = await _context.UserSessions
                .Where(s => s.UserId == userId)
                .ToListAsync();
            
            _context.UserSessions.RemoveRange(userSessions);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Başarıyla çıkış yapıldı" });
        }
    }
}
