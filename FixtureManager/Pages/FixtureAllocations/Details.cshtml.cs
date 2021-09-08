using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.FixtureAllocations
{
    public class DetailsModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public string FixtureDescription { get; set; }

        public DetailsModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public FixtureAllocation FixtureAllocation { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FixtureAllocation = await _context.FixtureAlloctation
                .Include(f => f.Fixture)
                    .ThenInclude(f => f.Team)
                .Include(f => f.Pitch)
                .Include(f => f.Referee).FirstOrDefaultAsync(m => m.Id == id);

            if (FixtureAllocation == null)
            {
                return NotFound();
            }

        FixtureDescription = $"{ FixtureAllocation.Fixture.Team.DisplayName} v { FixtureAllocation.Fixture.Opponent} on { FixtureAllocation.Fixture.Date.ToShortDateString()}";

    return Page();
        }
    }
}
