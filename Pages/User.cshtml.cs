using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Namespace.Models;
using System.Net.Http.Headers;

namespace MyApp.Namespace
{
    public class UserModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UserModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<User> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/Signin");
                return;
            }

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("http://localhost:5012/api/Users");

            if (response.IsSuccessStatusCode)
            {
                Users = await response.Content.ReadFromJsonAsync<List<User>>() ?? new();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Response.Redirect("/Signin");
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int userId, string username, string? password)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Signin");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var updateRequest = new
                {
                    Username = username,
                    Password = password
                };

                Console.WriteLine($"Updating user ID: {userId}");
                Console.WriteLine($"Request Body: {System.Text.Json.JsonSerializer.Serialize(updateRequest)}");

                var response = await client.PutAsJsonAsync(
                    $"http://localhost:5012/api/Users/{userId}",
                    updateRequest
                );

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User updated successfully!";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    TempData["ErrorMessage"] = "Access denied. You don't have permission to edit this user.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["ErrorMessage"] = "Session expired. Please login again.";
                    return RedirectToPage("/Signin");
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Body: {responseBody}");
                    TempData["ErrorMessage"] = $"Failed to update user. Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred.";
            }

            return RedirectToPage();
        }
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Signin");
        }
    }
}
