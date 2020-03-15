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
    public class RecordsTagsController : Controller
    {
        private readonly MoneyManagerDBContext _context;

        public RecordsTagsController(MoneyManagerDBContext context)
        {
            _context = context;
        }

        // GET: RecordsTags
        public async Task<IActionResult> Index()
        {
            var moneyManagerDBContext = _context.RecordsTags.Include(r => r.Record).Include(r => r.Tag);
            return View(await moneyManagerDBContext.ToListAsync());
        }

        // GET: RecordsTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordsTags = await _context.RecordsTags
                .Include(r => r.Record)
                .Include(r => r.Tag)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recordsTags == null)
            {
                return NotFound();
            }

            return View(recordsTags);
        }

        // GET: RecordsTags/Create
        //public IActionResult Create()
        //{
        //    ViewData["RecordId"] = new SelectList(_context.Records, "Id", "Id");
        //    ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Name");
        //    return View();
        //}

        public IActionResult Create(int? recordId)
        {
            //ViewData["RecordId"] = new SelectListItem("Id", id.ToString());
            ViewBag.RecordId = recordId;
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: RecordsTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RecordId,TagId")] RecordsTags recordsTags)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recordsTags);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecordId"] = new SelectList(_context.Records, "Id", "Id", recordsTags.RecordId);
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Name", recordsTags.TagId);
            return View(recordsTags);
        }

        // GET: RecordsTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordsTags = await _context.RecordsTags.FindAsync(id);
            if (recordsTags == null)
            {
                return NotFound();
            }
            ViewData["RecordId"] = new SelectList(_context.Records, "Id", "Id", recordsTags.RecordId);
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Name", recordsTags.TagId);
            return View(recordsTags);
        }

        // POST: RecordsTags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RecordId,TagId")] RecordsTags recordsTags)
        {
            if (id != recordsTags.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recordsTags);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordsTagsExists(recordsTags.Id))
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
            ViewData["RecordId"] = new SelectList(_context.Records, "Id", "Id", recordsTags.RecordId);
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Name", recordsTags.TagId);
            return View(recordsTags);
        }

        // GET: RecordsTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordsTags = await _context.RecordsTags
                .Include(r => r.Record)
                .Include(r => r.Tag)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recordsTags == null)
            {
                return NotFound();
            }

            return View(recordsTags);
        }

        // POST: RecordsTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recordsTags = await _context.RecordsTags.FindAsync(id);
            _context.RecordsTags.Remove(recordsTags);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordsTagsExists(int id)
        {
            return _context.RecordsTags.Any(e => e.Id == id);
        }
    }
}
