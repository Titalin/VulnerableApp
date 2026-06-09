using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using VulnerableApp.Data;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;

        public SearchController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string search)
        {
            if (string.IsNullOrEmpty(search))
                return View(new List<User>());

            // SEGURO: Uso de LINQ con expresiones lambda que genera consultas parametrizadas
            var users = _db.Users
                           .Where(u => u.Username.Contains(search))
                           .ToList();

            return View(users);
        }
    }
}