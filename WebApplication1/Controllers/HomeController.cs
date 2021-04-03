using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PersonContext personContext;

        private string ConnString = "Data source = DESKTOP-SS5TGJO\\SQLEXPRESS; Initial catalog = EFPerson; Integrated security = true;";

        public HomeController(ILogger<HomeController> logger, PersonContext personContext)
        {
            _logger = logger;
            this.personContext = personContext;
        }

        public async Task<IActionResult> Index()
        {
           
            return View(await personContext.People.ToListAsync());
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
       

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await personContext.People
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,FullName,EmpCode,Position,OfficeLocation")] Person person)
        {
            if (ModelState.IsValid)
            {
                personContext.Add(person);
                await personContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await personContext.People.FindAsync(id); //_context.Employees.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,FullName,EmpCode,Position,OfficeLocation")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    personContext.Update(person);//_context.Update(employee);
                    await personContext.SaveChangesAsync();//_context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId)) //EmployeeExists(employee.EmployeeId)
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
            return View(person);
        }

       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await personContext.People//_context.Employees
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await personContext.People.FindAsync(id);
            personContext.People.Remove(person);
            await personContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return personContext.People.Any(e => e.PersonId == id); 
        }
        [HttpGet]
        public IActionResult Scan()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetResultOfScan(string FullName)
        {
            using (IDbConnection db = new SqlConnection(ConnString))
            {
                return View(db.Query<Person>($"SELECT * FROM People WHERE (FullName Like '%{FullName}%')").ToList<Person>());
            }
        }
    }
}
