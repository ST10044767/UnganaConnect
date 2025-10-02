using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Controllers
{
    public class ModuleMvcController : Controller
    {
        private readonly UnganaConnectDbcontext _context;

        public ModuleMvcController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        // GET: /ModuleMvc
        public async Task<IActionResult> Index()
        {
            var modules = await _context.Modules
                .Include(m => m.Course)
                .ToListAsync();
            return View(modules);
        }

        // GET: /ModuleMvc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // GET: /ModuleMvc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ModuleMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,CourseId,VideoUrl,FileUrl")] Module module)
        {
            if (ModelState.IsValid)
            {
                _context.Add(module);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(module);
        }

        // GET: /ModuleMvc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound();
            }
            return View(module);
        }

        // POST: /ModuleMvc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CourseId,VideoUrl,FileUrl")] Module module)
        {
            if (id != module.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(module.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(module);
        }

        // GET: /ModuleMvc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // POST: /ModuleMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module != null)
            {
                _context.Modules.Remove(module);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }
    }
}
