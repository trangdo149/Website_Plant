using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website_Plant.MyHelpers;

namespace Website_Plant.Pages
{
    public class CallbackVnpayModel : PageModel
    {
        public void OnGet() { 
                ViewData["Status"] = true;
        }

    }
}
