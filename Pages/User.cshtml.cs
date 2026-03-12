using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Namespace.Models;

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
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5012/api/Users");

            if (response.IsSuccessStatusCode)
            {
                Users = await response.Content.ReadFromJsonAsync<List<User>>();
            }
        }

        // public void OnGet()
        // {
        // }
    }
}
