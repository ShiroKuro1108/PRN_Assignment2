using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace QuangThienDungRazorPages.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public LoginModel(ISystemAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // If user is already authenticated, redirect to appropriate dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Index");
            }

            // Clear any existing authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                SystemAccount? user = null;

                // Check if it's the admin account from appsettings
                var adminEmail = _configuration["AdminAccount:Email"];
                var adminPassword = _configuration["AdminAccount:Password"];
                var adminRole = _configuration.GetValue<int>("AdminAccount:Role");

                if (Input.Email == adminEmail && Input.Password == adminPassword)
                {
                    // Create admin user object
                    user = new SystemAccount
                    {
                        AccountID = 0, // Special ID for admin
                        AccountEmail = adminEmail,
                        AccountName = "System Administrator",
                        AccountRole = adminRole
                    };
                }
                else
                {
                    // Authenticate against database
                    user = await _accountService.AuthenticateAsync(Input.Email, Input.Password);
                }

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString()),
                        new Claim(ClaimTypes.Name, user.AccountName ?? ""),
                        new Claim(ClaimTypes.Email, user.AccountEmail ?? ""),
                        new Claim("Role", GetRoleName(user.AccountRole ?? 0))
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = Input.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToPage("/Index");
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }

        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Staff",
                2 => "Lecturer",
                3 => "Admin",
                _ => "Unknown"
            };
        }
    }
}
