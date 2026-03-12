using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using MyApp.Namespace.Models;

namespace UserDashboard.Pages;

public class SigninModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SigninModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public async Task OnGetAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        if (token != null)
        {
            Response.Redirect("/");
            return;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var client = _httpClientFactory.CreateClient();

            var loginRequest = new LoginRequest
            {
                Username = Username,
                Password = Password
            };

            var response = await client.PostAsJsonAsync(
                "http://localhost:5012/api/Auth/login",
                loginRequest
            );

            Console.WriteLine($"Response Status Code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    HttpContext.Session.SetString("Token", loginResponse.Token);
                    HttpContext.Session.SetString("Username", loginResponse.Username.ToString());
                    HttpContext.Session.SetString("IsLoggedIn", "true");

                    return RedirectToPage("/User");
                }
            }

            ErrorMessage = "Invalid username or password";
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Login failed. Please try again.";
            Console.WriteLine($"Login error: {ex.Message}");
            return Page();
        }
    }
}

