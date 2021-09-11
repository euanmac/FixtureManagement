using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Models;
using FixtureManager.Data;

namespace FixtureManager.Pages.People
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public IndexModel(ApplicationDBContext context)
        {
            _context = context;
        }

        public IList<Person> Person { get;set; }
        public string EmailAll { get; set; }

        public async Task OnGetAsync()
        {
            Person = await _context.Person
                .OrderBy(p => p.LastName)
                .ToListAsync();

            EmailAll = Person
                .Where(p => p.Email != null && p.Email.Length > 0 )
                .Select(p => p.Email)                
                .Aggregate("", (all, next) => all + "," + next);

        }
    }
}
