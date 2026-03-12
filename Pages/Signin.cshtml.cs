using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace UserDashboard.Pages;

public class SigninModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: Validasi login dengan API
        // Contoh sederhana:
        if (Username == "admin" && Password == "password")
        {
            // Login berhasil, redirect ke dashboard
            return RedirectToPage("/User");
        }

        ErrorMessage = "Invalid username or password";
        return Page();
    }
}

