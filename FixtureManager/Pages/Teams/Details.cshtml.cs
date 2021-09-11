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
    public class DetailsModel : PageModel
    {

        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public DetailsModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public Team Team { get; set; }
        public IList<Fixture> Fixtures { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Team = await _context.Team
                .Include(t => t.Contacts)
                    .ThenInclude(c => c.Person)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            Fixtures = await _context.Fixture
                .Where(f => f.TeamId == id)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Referee)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)              
                .OrderBy(f => f.Date)
                .AsNoTracking()
                .ToListAsync();

            if (Team != null)
            {
                return Page();
            }
            return NotFound();
        }
    }
}
