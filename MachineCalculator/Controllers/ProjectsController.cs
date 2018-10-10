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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;


using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;

namespace MachineCalculator.Controllers
{
    public class ProjectsController : Controller
    {
        

        private readonly ServerCapacityContext _context;
        private Calculator _calculator;
        private IHostingEnvironment _hostingEnvironment;

        public ProjectsController(ServerCapacityContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
            if (all.selected != null && all.selectedInst != null)
            {
                List<int> list = all.selected.Split(',').Select(Int32.Parse).ToList();
                List<int> numlist = all.selectedInst.Split(',').Select(Int32.Parse).ToList();
                temp.projectApps = new List<ProjectApp>();
                foreach (int current in list)
                {
                    temp.projectApps.Add(new ProjectApp() { appId = current, instances = numlist[list.IndexOf(current)] });
                }
                if (ModelState.IsValid)
                {
                    _context.Add(temp);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            
            all.apps = _context.AppObjDbSet.ToList();
            return View(all);
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
        public async Task<IActionResult> Edit(int id, [Bind("name,description")] Project project)
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
            _calculator = new Calculator(apps, 0);
            ViewBag.CurrentId = id;
            return View(_calculator.CalculateMedian());
        }

        public async Task<IActionResult> CalculateHard(int? id)
        {
            var project = await _context.ProjectDbSet.Include(s => s.projectApps)
               .FirstOrDefaultAsync(m => m.Id == id);
            List<CalcAppData> apps = new List<CalcAppData>();
            foreach (ProjectApp current in project.projectApps)
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
            _calculator = new Calculator(apps, 0);
            ViewBag.CurrentId = id;
            return View(_calculator.CalculateHardware());
        }

        public async Task<IActionResult> OnPostExport(int? id)
        {
            var project = await _context.ProjectDbSet.Include(s => s.projectApps)
                .FirstOrDefaultAsync(m => m.Id == id);
            List<CalcAppData> apps = new List<CalcAppData>();
            foreach (ProjectApp current in project.projectApps)
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
            _calculator = new Calculator(apps, 0);
            List<VM> vms = _calculator.CalculateMedian();

            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"Machines.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();

                ISheet excelSheet = workbook.CreateSheet("Services");
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("Services");
                row = excelSheet.CreateRow(row.RowNum + 1);
                row.CreateCell(0).SetCellValue("App name");
                row.CreateCell(1).SetCellValue("Instances");
                for (int i = 0; i < apps.First().resourses.Count(); i++)
                {
                    row.CreateCell(i + 2).SetCellValue(apps.First().resourses[i].name);
                    row.CreateCell(i + 2 + apps[0].resourses.Count()).SetCellValue(apps.First().resourses[i].name + " total");
                }

                for (int i = 0; i < apps.Count(); i++)
                {
                    row = excelSheet.CreateRow(i + 2);
                    row.CreateCell(0).SetCellValue(apps[i].name);
                    row.CreateCell(1).SetCellValue(apps[i].instances);
                    for (int j = 0; j < apps[i].resourses.Count(); j++)
                    {
                        row.CreateCell(j + 2).SetCellValue(apps[i].resourses[j].load);
                        row.CreateCell(j + 2 + apps[i].resourses.Count()).SetCellValue(apps[i].resourses[j].load * apps[i].instances);
                    }
                }

                List<double> totals = new List<double>();
                foreach(AppParameters current in apps[0].resourses)
                {
                    totals.Add(0);
                }

                for (int i = 0; i < apps[0].resourses.Count() ; i++)
                { 
                    for (int j = 0; j < apps.Count(); j++)
                    {
                       totals[i] += (apps[j].resourses[i].load * apps[j].instances);
                    }
                }

                row = excelSheet.CreateRow(row.RowNum + 1);
                row.CreateCell(1 + apps[0].resourses.Count()).SetCellValue("Totals: ");
                for (int i = 0; i < apps[0].resourses.Count; i++)
                {
                    row.CreateCell(i + 2 + apps[0].resourses.Count()).SetCellValue(totals[i]);
                }


                row = excelSheet.CreateRow(row.RowNum + 3);
                row.CreateCell(0).SetCellValue("Machines");
                row = excelSheet.CreateRow(row.RowNum + 1);
                for (int i = 0; i < vms.First().resourses.Count(); i++)
                {
                    row.CreateCell(0).SetCellValue("№");
                    row.CreateCell(i + 1).SetCellValue(vms.First().resourses[i].name);
                }

                for (int i = 0; i < vms.Count(); i++)
                {
                    row = excelSheet.CreateRow(row.RowNum + 1);
                    row.CreateCell(0).SetCellValue(i.ToString());
                    for (int j = 0; j < vms[i].resourses.Count(); j++)
                    {
                        row.CreateCell(j + 1).SetCellValue(vms[i].resourses[j].load);
                    }
                }

                row = excelSheet.CreateRow(row.RowNum + 3);
                row.CreateCell(0).SetCellValue("№");
                row.CreateCell(1).SetCellValue("Application");

                for (int i = 0; i < vms.Count(); i++)
                {
                    for (int j = 0; j < vms[i].runningApps.Count(); j++)
                    {
                        row = excelSheet.CreateRow(row.RowNum + 1);
                        row.CreateCell(0).SetCellValue(i.ToString());
                        row.CreateCell(1).SetCellValue(vms[i].runningApps[j]);
                    }
                }


                excelSheet = workbook.CreateSheet("Machines");
                row = excelSheet.CreateRow(0);

                for (int i = 0; i < vms.First().resourses.Count(); i++)
                {
                    row.CreateCell(i).SetCellValue(vms.First().resourses[i].name);
                }
                row.CreateCell(vms.First().resourses.Count()).SetCellValue("Running apps");

                for (int i = 0; i < vms.Count(); i++)
                {
                    row = excelSheet.CreateRow(i + 1);
                    for (int j = 0; j < vms[i].resourses.Count(); j++)
                    {
                        row.CreateCell(j).SetCellValue(vms[i].resourses[j].load);
                    }
                    row.CreateCell(vms[i].resourses.Count()).SetCellValue(String.Join(", ", vms[i].runningApps));
                }


                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        public async Task<IActionResult> OnPostExportHard(int? id)
        {
            var project = await _context.ProjectDbSet.Include(s => s.projectApps)
                .FirstOrDefaultAsync(m => m.Id == id);
            List<CalcAppData> apps = new List<CalcAppData>();
            foreach (ProjectApp current in project.projectApps)
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
            _calculator = new Calculator(apps, 0);
            List<VM> vms = _calculator.CalculateHardware();
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"Machines.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();

                ISheet excelSheet = workbook.CreateSheet("Services");
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("Services");
                row = excelSheet.CreateRow(row.RowNum + 1);
                row.CreateCell(0).SetCellValue("App name");
                row.CreateCell(1).SetCellValue("Instances");
                for (int i = 0; i < apps.First().resourses.Count(); i++)
                {
                    row.CreateCell(i + 2).SetCellValue(apps.First().resourses[i].name);
                    row.CreateCell(i + 2 + apps[0].resourses.Count()).SetCellValue(apps.First().resourses[i].name + " total");
                }

                for (int i = 0; i < apps.Count(); i++)
                {
                    row = excelSheet.CreateRow(i + 2);
                    row.CreateCell(0).SetCellValue(apps[i].name);
                    row.CreateCell(1).SetCellValue(apps[i].instances);
                    for (int j = 0; j < apps[i].resourses.Count(); j++)
                    {
                        row.CreateCell(j + 2).SetCellValue(apps[i].resourses[j].load);
                        row.CreateCell(j + 2 + apps[i].resourses.Count()).SetCellValue(apps[i].resourses[j].load * apps[i].instances);
                    }
                }

                List<double> totals = new List<double>();
                foreach (AppParameters current in apps[0].resourses)
                {
                    totals.Add(0);
                }

                for (int i = 0; i < apps[0].resourses.Count(); i++)
                {
                    for (int j = 0; j < apps.Count(); j++)
                    {
                        totals[i] += (apps[j].resourses[i].load * apps[j].instances);
                    }
                }

                row = excelSheet.CreateRow(row.RowNum + 1);
                row.CreateCell(1 + apps[0].resourses.Count()).SetCellValue("Totals: ");
                for (int i = 0; i < apps[0].resourses.Count; i++)
                {
                    row.CreateCell(i + 2 + apps[0].resourses.Count()).SetCellValue(totals[i]);
                }


                row = excelSheet.CreateRow(row.RowNum + 3);
                row.CreateCell(0).SetCellValue("Machines");
                row = excelSheet.CreateRow(row.RowNum + 1);
                for (int i = 0; i < vms.First().resourses.Count(); i++)
                {
                    row.CreateCell(0).SetCellValue("№");
                    row.CreateCell(i + 1).SetCellValue(vms.First().resourses[i].name);
                }

                for (int i = 0; i < vms.Count(); i++)
                {
                    row = excelSheet.CreateRow(row.RowNum + 1);
                    row.CreateCell(0).SetCellValue(i.ToString());
                    for (int j = 0; j < vms[i].resourses.Count(); j++)
                    {
                        row.CreateCell(j + 1).SetCellValue(vms[i].resourses[j].load);
                    }
                }

                row = excelSheet.CreateRow(row.RowNum + 3);
                row.CreateCell(0).SetCellValue("№");
                row.CreateCell(1).SetCellValue("Application");

                for (int i = 0; i < vms.Count(); i++)
                {
                    for (int j = 0; j < vms[i].runningApps.Count(); j++)
                    {
                        row = excelSheet.CreateRow(row.RowNum + 1);
                        row.CreateCell(0).SetCellValue(i.ToString());
                        row.CreateCell(1).SetCellValue(vms[i].runningApps[j]);
                    }
                }


                excelSheet = workbook.CreateSheet("Machines");
                row = excelSheet.CreateRow(0);

                for (int i = 0; i < vms.First().resourses.Count(); i++)
                {
                    row.CreateCell(i).SetCellValue(vms.First().resourses[i].name);
                }
                row.CreateCell(vms.First().resourses.Count()).SetCellValue("Running apps");

                for (int i = 0; i < vms.Count(); i++)
                {
                    row = excelSheet.CreateRow(i + 1);
                    for (int j = 0; j < vms[i].resourses.Count(); j++)
                    {
                        row.CreateCell(j).SetCellValue(vms[i].resourses[j].load);
                    }
                    row.CreateCell(vms[i].resourses.Count()).SetCellValue(String.Join(", ", vms[i].runningApps));
                }


                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
    }
}
