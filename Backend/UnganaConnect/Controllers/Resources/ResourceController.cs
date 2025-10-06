using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Service;

namespace UnganaConnect.Controllers.Resources
{

    [ApiController]
    [Route("api/[controller]")]



    public class ResourceController : Controller
    {

        private readonly FileServices fileServices;
        pr
        // GET: ResourceController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ResourceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ResourceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ResourceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ResourceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ResourceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ResourceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
