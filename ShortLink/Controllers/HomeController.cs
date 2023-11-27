using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortLink.Domain;
using ShortLink.Models;
using System.Diagnostics;

namespace ShortLink.Controllers
{
    public class HomeController : Controller
    {
        private  readonly ApplicationContext _db;

        public HomeController(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _db.Records.ToListAsync();

            return View(list);
        }

        public IActionResult AddRecord()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddRecord(RecordViewModel record)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return new UnprocessableEntityObjectResult(errors);
            }

            record.DateTime = DateTime.Now;
            record.ShortUrl = ModifyLink(record.LongUrl);
            await _db.Records.AddAsync(record);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteRecord(int id)
        {
            var record = await _db.Records.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            _db.Records.Remove(record);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ModifyRecord(int id)
        {
            var record = await _db.Records.FindAsync(id);
            if (record == null) 
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyRecord(RecordViewModel record)
        {
            record.DateTime = DateTime.Now;
            record.ClickCount = 0;
            record.ShortUrl = ModifyLink(record.LongUrl);

            _db.Update(record);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RedirectToLongUrl(int id)
        {
            var record = await _db.Records.FindAsync(id);

            if (record != null)
            {
                record.ClickCount++;
                await _db.SaveChangesAsync();

                return Redirect(record.LongUrl);
            }

            return NotFound();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string ModifyLink(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return string.Empty;
            }

            int firstSlashIndex = link.IndexOf('/');
            if (firstSlashIndex >= 0)
            {
                int secondSlashIndex = link.IndexOf('/', firstSlashIndex + 1);
                if (secondSlashIndex >= 0)
                {
                    int thirdSlashIndex = link.IndexOf('/', secondSlashIndex + 1);
                    if (thirdSlashIndex >= 0)
                    {
                        string domain = link.Substring(0, thirdSlashIndex);
                        string guid = Guid.NewGuid().ToString();
                        string firstPartOfGuid = guid.Split('-')[0];

                        string newLink = $"{domain}/{firstPartOfGuid}";

                        return newLink;
                    }
                }
            }

            return link;
        }
    }
}