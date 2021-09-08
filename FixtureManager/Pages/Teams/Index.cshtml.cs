using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.Teams
{
    public class IndexModel : PageModel
    {
#if DEBUG
        public bool Debug = true;
#else
        public bool Debug = false;
#endif
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public IndexModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; }

        public async Task OnGetAsync()
        {
            Team = await _context.Team
                .Include(t => t.Contacts)
                    .ThenInclude(c => c.Person)
                .OrderBy(t => t.AgeGroup)
                    .ThenBy(t => t.Name)
                .AsNoTracking()
                .ToListAsync();
                            
        }
    }
}
