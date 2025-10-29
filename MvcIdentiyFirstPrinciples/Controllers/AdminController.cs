using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcIdentiyFirstPrinciples.Controllers
{
    [Authorize(Roles = Roles.ADMIN_ROLE)]
    public class AdminController : Controller
    {
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

    }
}
