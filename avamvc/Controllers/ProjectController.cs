using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using avamvc.Models;

namespace avamvc.Controllers {
    [Authorize] //  �[�b�o�̡A��� ProjectController ���U�Ҧ��������ݭn�n�J�~���
    public class ProjectController : Controller {
        public IActionResult Index() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}