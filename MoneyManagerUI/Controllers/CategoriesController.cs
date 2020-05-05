using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace MoneyManagerUI.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly MoneyManagerDBContext _context;

        public CategoriesController(MoneyManagerDBContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string dataInputErrorMessage = "")
        {
            ViewBag.DataInputErrorMessage = dataInputErrorMessage;
            SelectList myList = new SelectList(_context.Tags, "Id", "Name");
            ViewBag.Tags = myList;
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Records", new { id = category.Id, name = category.Name });
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categories);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.FindAsync(id);
            if (categories == null)
            {
                return NotFound();
            }
            return View(categories);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Categories categories)
        {
            if (id != categories.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriesExists(categories.Id))
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
            return View(categories);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcats = _context.Subcategories.Where(sc => sc.CatedoryId == id);
            foreach (Subcategories subcat in subcats)
            {
                var records = _context.Records.Where(r => r.SubcategoryId == subcat.Id);
                foreach (Records record in records)
                {
                    var rt = _context.RecordsTags.Where(rt => rt.RecordId == record.Id);
                    _context.RecordsTags.RemoveRange(rt);
                }
                _context.Records.RemoveRange(records);
            }
            _context.Subcategories.RemoveRange(subcats);
            var categories = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(categories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddSubcat(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return RedirectToAction("Create", "Subcategories", new { id = category.Id, name = category.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                Categories category;
                                var sheetCategory = (from categ in _context.Categories
                                         where categ.Name.Contains(worksheet.Name)
                                         select categ).ToList();
                                if (sheetCategory.Count > 0)
                                {
                                    category = sheetCategory[0];
                                }
                                else
                                {
                                    category = new Categories() { Name = worksheet.Name };
                                    _context.Categories.Add(category);
                                }

                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        Records record = new Records();
                                        record.Sum = UInt32.Parse(row.Cell(1).GetString());
                                        record.Date = row.Cell(2).GetDateTime();

                                        Subcategories newSubcat;
                                        var subcatStr = row.Cell(3).GetString();
                                        var subcategory = (from subcat in _context.Subcategories
                                                           where subcat.Name.Contains(subcatStr)
                                                           select subcat).ToList();
                                        if (subcategory.Count > 0)
                                        {
                                            newSubcat = subcategory[0];
                                        }
                                        else
                                        {
                                            newSubcat = new Subcategories { Name = subcatStr };
                                            newSubcat.Catedory = category;
                                            _context.Subcategories.Add(newSubcat);
                                        }

                                        record.Category = category;
                                        record.Subcategory = newSubcat;
                                        _context.Records.Add(record);

                                        var tagNames = row.Cell(4).GetString().Split(", ");
                                        foreach (String tagName in tagNames)
                                        {
                                            Tags tag;
                                            var tags = (from T in _context.Tags
                                                        where T.Name.Contains(tagName)
                                                        select T).ToList();
                                            if (tags.Count > 0)
                                            {
                                                tag = tags[0];
                                            }
                                            else
                                            {
                                                tag = new Tags();
                                                tag.Name = tagName;
                                                _context.Add(tag);
                                                _context.SaveChanges();
                                            }

                                            RecordsTags rt = new RecordsTags();
                                            rt.Record = record;
                                            rt.Tag = tag;
                                            _context.RecordsTags.Add(rt);
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        return RedirectToAction("Index", "Categories", new { dataInputErrorMessage = "File contains invalid data. Upload hasn't been successful" });
                                    }
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool CategoriesExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
