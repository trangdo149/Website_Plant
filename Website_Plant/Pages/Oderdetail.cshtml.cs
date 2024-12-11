using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website_Plant.Pages
{
    public class Oderdetail : PageModel
    {
        private readonly ILogger<Oderdetail> _logger;

        public Oderdetail(ILogger<Oderdetail> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
