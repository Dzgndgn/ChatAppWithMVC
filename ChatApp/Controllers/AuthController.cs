using ChatApp.Context;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

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

            //bool isUserNameExist = await _context.users.AnyAsync(x => x.userName == dto.userName, cancellationToken);
            //if (isUserNameExist)
            //{
            //    ModelState.AddModelError("", "This username has been used");
            //    return View(dto);
            //}
            var isUserNameExist = await _userManager.FindByNameAsync(dto.userName);
            if(isUserNameExist != null)
            {
                ModelState.AddModelError("", "This username hass ben used");
            }
            var hasher = new PasswordHasher<User>();

            //var user = new User
            //{
            //    //userName = dto.userName,
            //    //password = dto.password 
            //    UserName = dto.userName,

            //    PasswordHash = hasher.HashPassword(user, dto.password)
            //};
            User user = new User();
            user.UserName = dto.userName;
            user.PasswordHash = hasher.HashPassword(user, dto.password);

            // await _context.AddAsync(user, cancellationToken);
            //await _context.SaveChangesAsync(cancellationToken);
            await _userManager.CreateAsync(user);
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

            //   var user = await _context.users.FirstOrDefaultAsync(x => x.userName == userName && x.password == password, cancellationToken);
            var signIn = await _signInManager.PasswordSignInAsync(userName, password, isPersistent: false, lockoutOnFailure: false);
            if (!signIn.Succeeded)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View();
            }

            //if (user is null)
            //{
            //    ModelState.AddModelError("", "Kullanıcı bulunamadı.");
            //    return View();
            //}
            var user =await _userManager.FindByNameAsync(userName);
            user.Status = "online";
            await _context.SaveChangesAsync(cancellationToken);

            
            TempData["AuthMessage"] = $"Hoş geldin, {user.UserName}!";
            return RedirectToAction("Index", "Chats", new
            {
                Id = user.Id
            });
        }

        
        
    }
}
