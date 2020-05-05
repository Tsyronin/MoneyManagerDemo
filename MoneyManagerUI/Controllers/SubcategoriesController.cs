using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace MoneyManagerUI.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> IndexByCategory(int id)
        {
            var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = category.Id;
            var subcatsByCat = _context.Subcategories.Where(s => s.CatedoryId == id).Include(s => s.Catedory);
            return View(await subcatsByCat.ToListAsync());
        }

        // GET: Subcategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _context.Subcategories
                .Include(s => s.Catedory)
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.CategoryId = subcategory.CatedoryId;
            if (subcategory == null)
            {
                return NotFound();
            }

            return View(subcategory);
        }

        // GET: Subcategories/Create
        public IActionResult Create(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Categories");

            ViewBag.CategoryId = id;
            ViewBag.CategoryName = name;

            return View();
        }

        // POST: Subcategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CatedoryId")] Subcategories subcategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subcategory);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexByCategory", new { id = subcategory.CatedoryId });
            }
            return RedirectToAction("IndexByCategory", new { id = subcategory.CatedoryId });
        }

        // GET: Subcategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _context.Subcategories.FindAsync(id);
            ViewBag.CategoryId = subcategory.CatedoryId;
            if (subcategory == null)
            {
                return NotFound();
            }
            //ViewData["CatedoryId"] = new SelectList(_context.Categories, "Id", "Name", subcategories.CatedoryId);
            return View(subcategory);
        }

        // POST: Subcategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CatedoryId")] Subcategories subcategory)
        {
            var categoryId = subcategory.CatedoryId;
            if (id != subcategory.Id)
            {
                return NotFound();
            }

            //var oldSubcategory = _context.Subcategories.Find(id);
            //_context.Entry<Subcategories>(oldSubcategory).State = EntityState.Detached;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subcategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubcategoriesExists(subcategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("IndexByCategory", new { id = categoryId });
            }
            ViewData["CatedoryId"] = new SelectList(_context.Categories, "Id", "Name", subcategory.CatedoryId);
            return View(subcategory);
        }

        // GET: Subcategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _context.Subcategories
                .Include(s => s.Catedory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subcategory == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = subcategory.CatedoryId;

            return View(subcategory);
        }

        // POST: Subcategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var records = _context.Records.Where(r => r.SubcategoryId == id);
            foreach (Records record in records)
            {
                var rt = _context.RecordsTags.Where(rt => rt.RecordId == record.Id);
                _context.RecordsTags.RemoveRange(rt);
            }
            _context.Records.RemoveRange(records);
            var subcategory = await _context.Subcategories.FindAsync(id);
            var categoryId = subcategory.CatedoryId;
            _context.Subcategories.Remove(subcategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexByCategory", new { id = categoryId });
        }

        private bool SubcategoriesExists(int id)
        {
            return _context.Subcategories.Any(e => e.Id == id);
        }
    }
}
