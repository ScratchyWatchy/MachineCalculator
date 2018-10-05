using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MachineCalculator.Models;
using X.PagedList;
using MachineCalculator.Business;

namespace MachineCalculator.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ServerCapacityContext _context;

        public ProjectsController(ServerCapacityContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, int perpage)
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

            var list = await _context.ProjectDbSet.ToListAsync();
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

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.ProjectDbSet.Include(s => s.projectApps)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            AllAppsAndProject AllAppsAndProject = new AllAppsAndProject
            {
                apps =_context.AppObjDbSet.ToList(),
            };
            
            return View(AllAppsAndProject);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AllAppsAndProject all)
        {
            Project temp = all.project;
            if (ModelState.IsValid)
            {
                List<int> list = all.selected.Split(',').Select(Int32.Parse).ToList();
                List<int> numlist = all.selectedInst.Split(',').Select(Int32.Parse).ToList();
                temp.projectApps = new List<ProjectApp>();
                foreach(int current in list)
                {
                    temp.projectApps.Add(new ProjectApp() { appId = current, instances = numlist[list.IndexOf(current)]});
                }
                _context.Add(temp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(temp);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.ProjectDbSet.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("name,description,Id")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.ProjectDbSet.Include(s => s.projectApps)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.ProjectDbSet.FindAsync(id);
            _context.ProjectDbSet.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.ProjectDbSet.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Calculate(int? id)
        {     
            var project = await _context.ProjectDbSet.Include(s => s.projectApps)
               .FirstOrDefaultAsync(m => m.Id == id);
            List<CalcAppData> apps = new List<CalcAppData>();
            foreach(ProjectApp current in project.projectApps)
            {
                var temp = await _context.AppObjDbSet.Include(s => s.AppParameters)
                    .FirstOrDefaultAsync(m => m.Id == current.appId);
                apps.Add(new CalcAppData()
                {
                    name = temp.name,
                    instances = current.instances,
                    resourses = temp.AppParameters
                });
            }
            Calculator cal = new Calculator(apps, 0);
            return View(cal.CalculateMedian());
        }
    }
}
