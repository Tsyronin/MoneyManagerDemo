using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace MoneyManagerUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChartsController : ControllerBase
    {
        private readonly MoneyManagerDBContext _context;

        public ChartsController(MoneyManagerDBContext context)
        {
            _context = context;
        }

        [HttpGet("JsonDataCatSum")]
        public JsonResult JsonDataCatSum()
        {
            var categories = _context.Categories.Include(c => c.Records).ToList();

            List<object> catSum = new List<object>();

            catSum.Add(new[] { "Category", "Sum" });

            foreach (var c in categories)
            {
                decimal sum = 0;
                foreach(var r in c.Records)
                {
                    sum += r.Sum;
                }
                catSum.Add(new object[] { c.Name, sum });
            }
            return new JsonResult(catSum);
        }

        [HttpGet("JsonDataDateSum")]
        public JsonResult JsonDataDateSum()
        {
            var categories = _context.Records.ToList();

            List<object> dateSum = new List<object>();

            dateSum.Add(new[] { "Date", "Spendings" });

            DateTime now = DateTime.Now;
            DateTime today = new System.DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            for (double daysBefore = 15; daysBefore > 0; --daysBefore)
            {
                decimal sum = 0;
                var recordsOfTheDay = _context.Records.Where(r => r.Date == today.AddDays(-daysBefore));
                foreach (var r in recordsOfTheDay)
                {
                    sum += r.Sum;
                }
                dateSum.Add(new object[] { today.AddDays(-daysBefore).ToString().Substring(0, 5), sum });
            }
            return new JsonResult(dateSum);
        }
    }
}