using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserDashboard.Pages;

public class IndexModel : PageModel
{
    public async Task OnGetAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            Response.Redirect("/Signin");
            return;
        }
    }
}
