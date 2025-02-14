using Microsoft.AspNetCore.Mvc;

namespace BloodCenter.Controllers
{
    public class DonorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
