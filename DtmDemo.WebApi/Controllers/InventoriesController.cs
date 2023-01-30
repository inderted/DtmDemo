using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DtmDemo.WebApi.Controllers
{
    public class InventoriesController : Controller
    {
        // GET: InventoriesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: InventoriesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InventoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InventoriesController/Create
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

        // GET: InventoriesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InventoriesController/Edit/5
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

        // GET: InventoriesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: InventoriesController/Delete/5
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
