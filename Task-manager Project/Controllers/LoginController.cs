using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_manager_Project.Data;
using Task_manager_Project.Models;

namespace Task_manager_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly TaskManagerDbContext _context;

        public LoginController(TaskManagerDbContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Password_hash,ConfirmPassword,role")] User user)
        {
            AesEncryptionService encryptionService = new AesEncryptionService();
            user.role = "User";
            user.created_at = DateTime.Now;
            user.last_login = DateTime.Now;
            user.Password_hash = encryptionService.Encrypt(encryptionService.Encrypt(encryptionService.Encrypt(encryptionService.Encrypt(encryptionService.Encrypt(user.Password_hash)))));
            user.ConfirmPassword = encryptionService.Encrypt(encryptionService.Encrypt(encryptionService.Encrypt(encryptionService.Encrypt(encryptionService.Encrypt(user.ConfirmPassword)))));
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Home");
            }
            return View(user);
        }
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            var decryptionservice = new AesEncryptionService();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                ViewBag.Error = "User not Found";
                return View();
            }
            try
            {
                var decryptedPassword = decryptionservice.Decrypt(decryptionservice.Decrypt(decryptionservice.Decrypt(decryptionservice.Decrypt(decryptionservice.Decrypt(user.Password_hash)))));

                if (decryptedPassword == password)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.Name),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe,
                        ExpiresUtc = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

                    HttpContext.Session.SetString("Name", user.Name);
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Role",user.role);

                    ViewBag.UserName= HttpContext.Session.GetString("Name");
                    ViewBag.UID = HttpContext.Session.GetString("UserId");
                }
                else
                {
                    ViewBag.Error = "Wrong Password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Somthing Went Worng";
                return View();
            }
            user.last_login = DateTime.UtcNow;
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
