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
using Microsoft.AspNetCore.Authorization;
using MoneyManagerUI.ViewModel;

namespace MoneyManagerUI.Controllers
{
    [Authorize]
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
                                        .Include(r => r.Subcategory)
                                        .OrderByDescending(r => r.Date);
            return View(await RecordsByCategory.ToListAsync());
        }

        [Authorize(Roles = "admin, premiumUser")]
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
                            .Include(r => r.Subcategory)
                            .OrderByDescending(r => r.Date);

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
        public IActionResult Create(int Id)
        {
            ViewBag.CategoryId = Id;
            ViewBag.CategoryName = _context.Categories.Where(c => c.Id == Id).FirstOrDefault().Name;
            var subcatList = _context.Subcategories.Where(s => s.CatedoryId == Id).ToList();
            ViewBag.Subcategories = new SelectList(subcatList, "Id", "Name");

            var model = new RecordViewModel();

            var tags = _context.Tags.Select(c => new
            {
                TagId = c.Id,
                TagName = c.Name
            }).ToList();
            model.Tags = new MultiSelectList(tags, "TagId", "TagName");
            model.TagIds = new[] { 1, 3 };



            //MultiSelectList myList = new MultiSelectList(_context.Tags, "Id", "Name");
            //ViewBag.Tags = myList;

            return View(model);
        }

        // POST: Records/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int categoryId, RecordViewModel recordVM)
        {
            var record = new Records()
            {
                Sum = recordVM.Sum,
                CategoryId = recordVM.CategoryId,
                SubcategoryId = recordVM.SubcategoryId,
                Date = DateTime.Now
            };
            _context.Add(record);

            await _context.SaveChangesAsync();

            foreach (var item in recordVM.TagIds)
            {
                var recordTag = new RecordsTags()
                {
                    RecordId = record.Id,
                    TagId = item
                };
                _context.Add(recordTag);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Records", new { id = categoryId, name = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault().Name });
        }

        // GET: Records/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.Records.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = record.CategoryId;

            var subcatList = _context.Subcategories.Where(s => s.CatedoryId == record.CategoryId).ToList();
            ViewBag.Subcategories = new SelectList(subcatList, "Id", "Name");

            var model = new RecordViewModel();

            var tags = _context.Tags.Select(c => new
            {
                TagId = c.Id,
                TagName = c.Name
            }).ToList();
            model.Tags = new MultiSelectList(tags, "TagId", "TagName");
            model.Id = id;

            model.TagIds = _context.RecordsTags.Where(rt => rt.RecordId == id).Select(rt => rt.TagId).ToArray(); ;

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", record.CategoryId);
            ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "Id", "Name", record.SubcategoryId);
            return View(model);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecordViewModel recordVM)
        {
            if (id != recordVM.Id)
            {
                return NotFound();
            }

            var oldTagIds = _context.RecordsTags.Where(rt => rt.RecordId == id).Select(rt => rt.TagId).ToList();
            var newTagIds = recordVM.TagIds.ToList();
            var addedTagIds = newTagIds.Except(oldTagIds);
            var removedTagIds = oldTagIds.Except(newTagIds);

            foreach(int newTagId in addedTagIds)
            {
                RecordsTags recordTag = new RecordsTags()
                {
                    RecordId = id,
                    TagId = newTagId
                };
                _context.RecordsTags.Add(recordTag);
            }
            foreach (int removedTagId in removedTagIds)
            {
                var removedTags = _context.RecordsTags.Where(rt => ((rt.RecordId == id) && (rt.TagId == removedTagId))).ToList();
                _context.RecordsTags.RemoveRange(removedTags);
            }

            var oldRecord = _context.Records.Find(id);

            Records UpRecord = new Records()
            {
                Id = id,
                Sum = recordVM.Sum,
                CategoryId = oldRecord.CategoryId,
                SubcategoryId = recordVM.SubcategoryId,
                Date = oldRecord.Date
            };

            _context.Entry<Records>(oldRecord).State = EntityState.Detached;

            try
            {
                _context.Records.Update(UpRecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordsExists(UpRecord.Id))
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

        //public async Task<IActionResult> AddTag(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var records = await _context.Records.FindAsync(id);
        //    if (records == null)
        //    {
        //        return NotFound();
        //    }
        //    return RedirectToAction("Create", "RecordsTags", new { recordId = records.Id });
        //}

        public ActionResult Export(int categoryId)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var category = _context.Categories.Find(categoryId);

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
