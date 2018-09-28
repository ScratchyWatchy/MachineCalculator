using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MachineCalculator.Models;
using UserDBWebRest.Business;
using X.PagedList;

namespace MachineCalculator.Controllers
{
    public class AppDatasController : Controller
    {
        private readonly AppDataContext _context;

        public AppDatasController(AppDataContext context)
        {
            _context = context;
        }

        // GET: AppDatas
        public async Task<IActionResult> Index(string sortOrder,string currentFilter, string searchString, int? page, int perpage)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PerPageParam = perpage == 0 ? 10 : perpage;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var list = await _context.AppDatas.Include(s => s.resourses).ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(s => s.name.Contains(searchString)).ToList();
            }
                switch (sortOrder)
            {
                case "name_desc":
                    list = list.OrderByDescending(s => s.name).ToList();
                    break;
                default:
                    list = list.OrderBy(s => s.name).ToList();
                    break;
            }
            int pageSize = ViewBag.PerPageParam;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        // GET: AppDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appData = await _context.AppDatas
                .Include(s => s.resourses)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appData == null)
            {
                return NotFound();
            }

            return View(appData);
        }

        // GET: AppDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RAM,CPU,name,instances,flag,Id")] AppData appData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appData);
        }

        // GET: AppDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appData = await _context.AppDatas
                .Include(s => s.resourses)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (appData == null)
            {
                return NotFound();
            }
            return View(appData);
        }

        // POST: AppDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RAM,CPU,name,instances,flag,Id")] AppData appData)
        {
            if (id != appData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppDataExists(appData.Id))
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
            return View(appData);
        }

        // GET: AppDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appData = await _context.AppDatas
                .Include(s => s.resourses)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appData == null)
            {
                return NotFound();
            }

            return View(appData);
        }

        // POST: AppDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appData = await _context.AppDatas.FindAsync(id);
            _context.AppDatas.Remove(appData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppDataExists(int id)
        {
            return _context.AppDatas.Any(e => e.Id == id);
        }
    }
}
