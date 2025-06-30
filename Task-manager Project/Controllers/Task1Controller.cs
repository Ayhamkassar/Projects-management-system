using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Task_manager_Project.Data;
using Task_manager_Project.Models;

namespace Task_manager_Project.Controllers
{
    public class Task1Controller : Controller
    {
        private readonly TaskManagerDbContext _context;

        public Task1Controller(TaskManagerDbContext context)
        {
            _context = context;
        }

        // GET: Task1
        public async Task<IActionResult> Index()
        {
            var logging = HttpContext.Session.GetString("IsLoggedIn");
            if (logging == "true")
            {
                var userId = HttpContext.Session.GetString("UserId");
                var taskManagerDbContext = _context.Tasks
                    .Include(t => t.Project)
                    .Include(t => t.AssignedTo)
                    .Where(t => t.Project.owner_id.ToString() == userId || t.assignedToId.ToString() == userId)
                    .ToList();
                return View(taskManagerDbContext);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Task1/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task1 = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task1 == null)
            {
                return NotFound();
            }

            return View(task1);
        }

        // GET: Task1/Create
        public IActionResult Create()
        {
            ViewData["Projectid"] = new SelectList(_context.Projects, "Id", "Name");
            PopulateUsersDropDown();
            return View();
        }
        private void PopulateUsersDropDown()
        {
            var users = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();
            ViewBag.Users = users;
        }
        // POST: Task1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Projectid,Status,Priority,Due_date,Created_at")] Task1 task1,Guid assignto)
        {
            var selectid = _context.Users.FirstOrDefault(s => s.Id == assignto);
            if (selectid != null)
            {
                task1.assignedToId = selectid.Id;
            }
            if (ModelState.IsValid)
            {
                _context.Add(task1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var model in ModelState)
                {
                    foreach (var item in model.Value.Errors)
                    {
                        Console.WriteLine($"Field : {model.Key}, Error : {item.ErrorMessage}");
                    }
                }
            }
            ViewData["Projectid"] = new SelectList(_context.Projects, "Id", "Name", task1.Projectid);
            PopulateUsersDropDown();

            Console.WriteLine(task1.assignedToId);
            return View(task1);
        }

        // GET: Task1/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task1 = await _context.Tasks.FindAsync(id);
            if (task1 == null)
            {
                return NotFound();
            }
            ViewData["Projectid"] = new SelectList(_context.Projects, "Id", "Name", task1.Projectid);
            PopulateUsersDropDown();
            return View(task1);
        }

        // POST: Task1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Projectid,Status,Priority,Due_date,Created_at")] Task1 task1,Guid assignto)
        {
            if (id != task1.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    task1.assignedToId = assignto;
                    _context.Update(task1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Task1Exists(task1.Id))
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
            ViewData["Projectid"] = new SelectList(_context.Projects, "Id", "Name", task1.Projectid);
            return View(task1);
        }

        // GET: Task1/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task1 = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task1 == null)
            {
                return NotFound();
            }

            return View(task1);
        }

        // POST: Task1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var task1 = await _context.Tasks.FindAsync(id);
            if (task1 != null)
            {
                _context.Tasks.Remove(task1);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Completed(Guid? id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
                return NotFound();
            task.IsCompleted = true;
            task.Status = "Complete";
            _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool Task1Exists(Guid id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
