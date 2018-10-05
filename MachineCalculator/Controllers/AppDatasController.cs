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
        private readonly ServerCapacityContext _context;

        public AppDatasController(ServerCapacityContext context)
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

            var list = await _context.AppObjDbSet.Include(s => s.AppParameters).ToListAsync();
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

            var appData = await _context.AppObjDbSet
                .Include(s => s.AppParameters)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appData == null)
            {
                return NotFound();
            }

            return View(appData);
        }

        // GET: AppDatas/Create
        public async Task<IActionResult> Create()
        {
            var appData = await _context.AppObjDbSet
                .Include(s => s.AppParameters)
                .FirstOrDefaultAsync(e => e.Id == 1);
            return View(appData);
        }

        // POST: AppDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("name,flag,load")] AppObj appData, List<AppParameters> appParameters)
        {
            if (ModelState.IsValid)
            {
                appData.AppParameters = appParameters;
                _context.Add(appData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            appData.AppParameters = appParameters;
            return View(appData);
        }

        // GET: AppDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appData = await _context.AppObjDbSet
                .Include(s => s.AppParameters)
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
        public async Task<IActionResult> Edit(int id, [Bind("name,flag,Id")] AppObj appData, List<AppParameters> appParameters)
        {
            if (id != appData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appData.AppParameters = appParameters;
                    var param = _context.AppParameterDbSet.Where(e => e.AppId == appData.Id);
                    foreach(var current in param)
                    {
                        _context.AppParameterDbSet.Remove(current);
                    }                 
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

            var appData = await _context.AppObjDbSet
                .Include(s => s.AppParameters)
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
            var appData = await _context.AppObjDbSet.FindAsync(id);
            _context.AppObjDbSet.Remove(appData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppDataExists(int id)
        {
            return _context.AppObjDbSet.Any(e => e.Id == id);
        }
    }
}
