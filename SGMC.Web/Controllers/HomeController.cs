//using Microsoft.AspNetCore.Mvc;

//namespace SGMC.Web.Controllers
//{
//    public class HomeController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGMC.Persistence.Context;

namespace SGMC.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HealtSyncContext _context;

        public HomeController(HealtSyncContext context)
        {
            _context = context;
        }

        // Acción de prueba
        public IActionResult DbInfo()
        {
            var conn = _context.Database.GetDbConnection();
            return Content($"Server: {conn.DataSource} | Database: {conn.Database}");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
