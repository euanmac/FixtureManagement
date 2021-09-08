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
    public class IndexModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public IndexModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IList<FixtureAllocation> FixtureAllocation { get;set; }

        public async Task OnGetAsync()
        {
            FixtureAllocation = await _context.FixtureAlloctation
                .Include(f => f.Fixture)
                    .ThenInclude(f => f.Team)
                .Include(f => f.Pitch)
                .Include(f => f.Referee).ToListAsync();
        }
    }
}
