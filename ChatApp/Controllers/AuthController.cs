using ChatApp.Context;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    
    public class AuthController(AppDbContext _context) : Controller
    {
       
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Views/Auth/Register.cshtml
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] registerDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(dto);

            bool isUserNameExist = await _context.users.AnyAsync(x => x.userName == dto.userName, cancellationToken);
            if (isUserNameExist)
            {
                ModelState.AddModelError("", "This username has been used");
                return View(dto);
            }

            var user = new User
            {
                userName = dto.userName,
                password = dto.password 
            };

            await _context.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            TempData["AuthMessage"] = "Kayıt başarılı. Giriş yapabilirsiniz.";
            return RedirectToAction(nameof(Login));
        }

        
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Views/Auth/Login.cshtml
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] string userName, [FromForm] string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Kullanıcı adı ve şifre zorunludur.");
                return View();
            }

            var user = await _context.users.FirstOrDefaultAsync(x => x.userName == userName && x.password == password, cancellationToken);
            if (user is null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View();
            }

            user.Status = "online";
            await _context.SaveChangesAsync(cancellationToken);

            
            TempData["AuthMessage"] = $"Hoş geldin, {user.userName}!";
            return RedirectToAction("Index", "Chats", new
            {
                Id = user.Id
            });
        }

        
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> RegisterApi([FromBody] registerDto dto, CancellationToken cancellationToken)
        {
            bool isUserNameExist = await _context.users.AnyAsync(x => x.userName == dto.userName, cancellationToken);
            if (isUserNameExist)
                return BadRequest(new { Message = "This username has been used" });

            var user = new User { userName = dto.userName, password = dto.password };
            await _context.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        
        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> LoginApi(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.userName == username && x.password == password, cancellationToken);
            if (user is null)
                return BadRequest(new { Message = "Kullanıcı bulunamadı." });

            user.Status = "online";
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(user);
        }
    }
}
