using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.TeamContacts
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

        public IList<TeamContact> TeamContact { get;set; }

        public async Task OnGetAsync()
        {
            TeamContact = await _context.TeamContact
                .Include(t => t.Person)
                .OrderBy(t => t.Team.AgeGroup)
                .ThenBy(t => t.Team.Gender)
                .Include(t => t.Team).ToListAsync();
        }
    }
}
