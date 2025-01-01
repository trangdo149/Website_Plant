using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages
{
    [RequireAuth(RequiredRole = "client")]
    public class thankyouModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
