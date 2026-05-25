using System.Web.Mvc;

namespace QuanLyDatLichKhamBenh.Controllers
{
    public class TrangChuController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Trang chu";
            return View();
        }
    }
}