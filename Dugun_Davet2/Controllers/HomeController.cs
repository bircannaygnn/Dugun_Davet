using Dugun_Davet2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dugun_Davet2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ViewResult CevapFormu()
        {
            return View();
        }
        [HttpPost]
        public ViewResult CevapFormu(Cevap KatilimciCevabi)
        {
            if (ModelState.IsValid)
            {
                Repository.CevapEkle(KatilimciCevabi);
                return View("Tesekkurler", KatilimciCevabi);
            }
            else
            {
                return View();

            }
        }
        public ViewResult CevaplariListele()

        {
            return View(Repository.Cevaplar.Where(x => x.DuguneGelecekmi == true));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
