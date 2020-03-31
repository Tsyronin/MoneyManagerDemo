using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyManagerUI;

namespace MoneyManagerUI.Controllers
{
    public class RecordsController : Controller
    {
        private readonly MoneyManagerDBContext _context;

        public RecordsController(MoneyManagerDBContext context)
        {
            _context = context;
        }

        // GET: Records
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Categories");

            ViewBag.CategoryId = id;
            ViewBag.CategoryName = name;
            var RecordsByCategory = _context.Records
                                        .Where(r => r.CategoryId == id)
                                        .Include(r => r.Category)
                                        .Include(r => r.Subcategory);
            return View(await RecordsByCategory.ToListAsync());
        }

        public async Task<IActionResult> IndexByTag(int? tag)
        {
            if (tag == null) return RedirectToAction("Index", "Categories");

            ViewBag.TagId = tag;
            ViewBag.TagName = _context.Tags.Find(tag).Name;

            var recordIDs = _context.RecordsTags
                            .Where(rt => rt.TagId == tag)
                            .Select(rt => rt.RecordId).ToList();
            var RecordsByTag = _context.Records
                            .Where(r => recordIDs.Contains(r.Id))
                            .Include(r => r.Category)
                            .Include(r => r.Subcategory);

            return View(await RecordsByTag.ToListAsync());
        }



        // GET: Records/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var records = await _context.Records
                .Include(r => r.Category)
                .Include(r => r.Subcategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (records == null)
            {
                return NotFound();
            }

            return View(records);
        }

        // GET: Records/Create
        public IActionResult Create(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault().Name;
            var subcatList = _context.Subcategories.Where(s => s.CatedoryId == categoryId).ToList();
            ViewBag.Subcategories = new SelectList(subcatList, "Id", "Name");

            //For creating tags
            //var myList = new List<SelectListItem>();
            //foreach (var item in _context.Tags)
            //{
            //    myList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString()/*, Selected = false*/ });
            //}
            MultiSelectList myList = new MultiSelectList(_context.Tags, "Id", "Name");
            ViewBag.Tags = myList;

            return View();
        }

        // POST: Records/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int categoryId, [Bind("Id,Sum,CategoryId,SubcategoryId,Date")] Records record)
        {
            record.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(record);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Records", new { id = categoryId, name = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault().Name });
            }
            return RedirectToAction("Index", "Records", new { id = categoryId, name = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault().Name });
        }

        // GET: Records/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var records = await _context.Records.FindAsync(id);
            if (records == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", records.CategoryId);
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "Id", "Name", records.SubcategoryId);
            return View(records);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sum,CategoryId,SubcategoryId,Date")] Records records)
        {
            if (id != records.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(records);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordsExists(records.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", records.CategoryId);
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "Id", "Name", records.SubcategoryId);
            return View(records);
        }

        // GET: Records/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var records = await _context.Records
                .Include(r => r.Category)
                .Include(r => r.Subcategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (records == null)
            {
                return NotFound();
            }

            return View(records);
        }

        // POST: Records/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rt = _context.RecordsTags.Where(rt => rt.RecordId == id);
            _context.RecordsTags.RemoveRange(rt);
            var records = await _context.Records.FindAsync(id);
            _context.Records.Remove(records);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddTag(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var records = await _context.Records.FindAsync(id);
            if (records == null)
            {
                return NotFound();
            }
            return RedirectToAction("Create", "RecordsTags", new { recordId = records.Id });
        }

        public ActionResult Export(int categoryId)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var category = _context.Categories.Find(categoryId);
                //тут, для прикладу ми пишемо усі книжки з БД, в своїх проектах ТАК НЕ РОБИТИ (писати лише вибрані)

                var worksheet = workbook.Worksheets.Add(category.Name);

                worksheet.Cell("A1").Value = "Sum";
                worksheet.Cell("B1").Value = "Date";
                worksheet.Cell("C1").Value = "Subcategory";
                worksheet.Cell("D1").Value = "Tags";
                worksheet.Row(1).Style.Font.Bold = true;
                var records = _context.Records.Where(r => r.CategoryId == categoryId).ToList();

                for (int i = 0; i < records.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = records[i].Sum;
                    worksheet.Cell(i + 2, 2).Value = records[i].Date.ToString();
                    var subcatId = records[i].SubcategoryId;
                    worksheet.Cell(i + 2, 3).Value = _context.Subcategories.Find(subcatId).Name;

                    var rt = _context.RecordsTags.Where(item => item.RecordId == records[i].Id).Select(item => item.Tag.Name).ToList();
                    var tagStr = String.Join(", ", rt);
                    worksheet.Cell(i + 2, 4).Value = tagStr;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"MM_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

        private bool RecordsExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }
    }
}
