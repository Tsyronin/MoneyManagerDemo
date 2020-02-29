﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                                        .Where(r=>r.CategoryId == id)
                                        .Include(r => r.Category)
                                        .Include(r => r.Subcategory);
            return View(await RecordsByCategory.ToListAsync());
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
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            //ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "Id", "Name");

            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault().Name;
            return View();
        }

        // POST: Records/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int categoryId, [Bind("Id,Sum,CategoryId,SubcategoryId,Date")] Records records)
        {
            if (ModelState.IsValid)
            {
                _context.Add(records);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                //
                //
                //
                //Might need some changes
                return RedirectToAction("Index", "Records", new { id = categoryId, name = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault().Name });
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", records.CategoryId);
            //ViewData["SubcategoryId"] = new SelectList(_context.Subcategories, "Id", "Name", records.SubcategoryId);
            //return View(records);
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
            var records = await _context.Records.FindAsync(id);
            _context.Records.Remove(records);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecordsExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }
    }
}