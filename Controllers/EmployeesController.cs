using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesApp.Data;
using EmployeesApp.Models;

namespace EmployeesApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString)
        {
            var employees = from e in _context.Employees
                            select e;

            // Filter by Name if searchString is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Name.Contains(searchString) || e.LastName.Contains(searchString));
            }

            // Sort by BornDate
            employees = employees.OrderBy(e => e.BornDate);

            return View(await employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,RFC,BornDate,Status")] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            // Validate uniqueness of RFC
            if (_context.Employees.Any(e => e.RFC == employee.RFC))
            {
                ModelState.AddModelError("RFC", "An employee with this RFC already exists.");
                return View(employee);
            }

            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Populate the Status options
            ViewBag.StatusOptions = Enum.GetValues(typeof(EmployeeStatus)).Cast<EmployeeStatus>();

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,RFC,BornDate,Status")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if the RFC is unique, but ignore the current employee's RFC
                var existingEmployeeWithRFC = _context.Employees
                    .FirstOrDefault(e => e.RFC == employee.RFC && e.Id != employee.Id);

                if (existingEmployeeWithRFC != null)
                {
                    ModelState.AddModelError("RFC", "An employee with this RFC already exists.");
                    // Repopulate the Status options
                    ViewBag.StatusOptions = Enum.GetValues(typeof(EmployeeStatus)).Cast<EmployeeStatus>();
                    return View(employee);
                }

                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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

            // Repopulate the Status options
            ViewBag.StatusOptions = Enum.GetValues(typeof(EmployeeStatus)).Cast<EmployeeStatus>();
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
