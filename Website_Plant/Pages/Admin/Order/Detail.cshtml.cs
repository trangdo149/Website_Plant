using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages.Admin.Order
{
    [RequireAuth(RequiredRole = "admin")]
    public class DetailModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
