using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website_Plant.Pages
{
    public class Oder : PageModel
    {
        private readonly ILogger<Oder> _logger;

        public Oder(ILogger<Oder> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
