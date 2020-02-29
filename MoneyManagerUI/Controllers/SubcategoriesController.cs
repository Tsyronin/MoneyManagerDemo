using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyManagerUI;

namespace MoneyManagerUI.Controllers
{
    public class SubcategoriesController : Controller
    {
        private readonly MoneyManagerDBContext _context;

        public SubcategoriesController(MoneyManagerDBContext context)
        {
            _context = context;
        }

        // GET: Subcategories
        public async Task<IActionResult> Index()
        {
            var moneyManagerDBContext = _context.Subcategories.Include(s => s.Catedory);
            return View(await moneyManagerDBContext.ToListAsync());
        }

        // GET: Subcategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = await _context.Subcategories
                .Include(s => s.Catedory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subcategories == null)
            {
                return NotFound();
            }

            return View(subcategories);
        }

        // GET: Subcategories/Create
        public IActionResult Create()
        {
            ViewData["CatedoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Subcategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CatedoryId")] Subcategories subcategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subcategories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatedoryId"] = new SelectList(_context.Categories, "Id", "Name", subcategories.CatedoryId);
            return View(subcategories);
        }

        // GET: Subcategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = await _context.Subcategories.FindAsync(id);
            if (subcategories == null)
            {
                return NotFound();
            }
            ViewData["CatedoryId"] = new SelectList(_context.Categories, "Id", "Name", subcategories.CatedoryId);
            return View(subcategories);
        }

        // POST: Subcategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CatedoryId")] Subcategories subcategories)
        {
            if (id != subcategories.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subcategories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubcategoriesExists(subcategories.Id))
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
            ViewData["CatedoryId"] = new SelectList(_context.Categories, "Id", "Name", subcategories.CatedoryId);
            return View(subcategories);
        }

        // GET: Subcategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = await _context.Subcategories
                .Include(s => s.Catedory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subcategories == null)
            {
                return NotFound();
            }

            return View(subcategories);
        }

        // POST: Subcategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcategories = await _context.Subcategories.FindAsync(id);
            _context.Subcategories.Remove(subcategories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubcategoriesExists(int id)
        {
            return _context.Subcategories.Any(e => e.Id == id);
        }
    }
}
