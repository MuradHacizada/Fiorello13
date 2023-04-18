using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Areas.Admin.Views
{
    [Area("Admin")]
    public class DashboardController : Controller
    {        
        public IActionResult Index()
        {

            return View();
        }
    }
}
