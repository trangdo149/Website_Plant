using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages.Admin.Order
{
    [RequireAuth(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
