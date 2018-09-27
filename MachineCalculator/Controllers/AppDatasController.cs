﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MachineCalculator.Models;
using UserDBWebRest.Business;

namespace MachineCalculator.Controllers
{
    public class AppDatasController : Controller
    {
        private readonly AppDataContext _context;
        private PostgrsqlRepository repo;

        public AppDatasController(AppDataContext context)
        {
            _context = context;
        }

        // GET: AppDatas
        public async Task<IActionResult> Index()
        {
            repo = new PostgrsqlRepository(_context);
            List<AppData> apps = new List<AppData>();
            apps.Add(new AppData("Nginx 1", 1, new List<Parameter>
             {
                 new Parameter("Ram", 1),
                 new Parameter("CPU", 4)
             }));
            repo.Create(apps[0]);
            return View(await _context.AppDatas.Include(s => s.resourses).ToListAsync());
        }

        // GET: AppDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appData = await _context.AppDatas
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

            var appData = await _context.AppDatas.FindAsync(id);
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