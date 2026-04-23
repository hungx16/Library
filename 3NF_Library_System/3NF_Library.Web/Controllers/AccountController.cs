using _3NF_Library.Data.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _3NF_Library.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryDbContext _context;
        public AccountController(LibraryDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string accountType, string username, string password, string? secretCode)
        {
            // Phiên dịch vai trò cho SQL
            string sqlAccountType = accountType == "Librarian" ? "Thủ thư" : (accountType == "Reader" ? "Độc giả" : accountType);

            var result = await _context.Procedures.sp_LogInAsync(sqlAccountType, username, password);

            if (result != null && result.Count > 0)
            {
                var userInfo = result.First();

                // Kiểm tra mã bảo mật nội bộ
                if (userInfo.RoleType == "Admin" && secretCode != "987654321A")
                {
                    ViewBag.ErrorMessage = "Mã bảo mật Admin sai!"; return View();
                }
                if (userInfo.RoleType == "Librarian" && secretCode != "123456789L")
                {
                    ViewBag.ErrorMessage = "Mã bảo mật Thủ thư sai!"; return View();
                }

                TempData["SuccessMessage"] = $"Xin chào {userInfo.FullName}!\nĐăng nhập thành công vai trò {userInfo.RoleType}.";

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.AccountID.ToString()),
                    new Claim(ClaimTypes.Name, userInfo.FullName),
                    new Claim(ClaimTypes.Role, userInfo.RoleType)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
                // ĐIỀU HƯỚNG DỰA TRÊN VAI TRÒ (ROLE)
                if (userInfo.RoleType == "Admin")
                    return RedirectToAction("AdminDashboard", "Home");
                else if (userInfo.RoleType == "Librarian")
                    return RedirectToAction("LibrarianDashboard", "Home");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}