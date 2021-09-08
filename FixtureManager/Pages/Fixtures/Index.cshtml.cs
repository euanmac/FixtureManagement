using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;
using System.ComponentModel.DataAnnotations;

namespace FixtureManager.Pages.Fixtures
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
            IncludeHomeFilter = true;
            IncludeAwayFilter = false;
            StartDateFilter = DateTime.Now.Date;
            EndDateFilter = DateTime.Now.Date.AddDays(21);
        }

        public IList<Fixture> Fixture { get;set; }

        [BindProperty(SupportsGet = true)]
        public Boolean IncludeHomeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public Boolean IncludeAwayFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime StartDateFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime EndDateFilter { get; set; }


        public async Task OnGetAsync()
        {
            //bool includeHome = VenueFilter != "A";
            //bool includeAway = VenueFilter != "H";

             Fixture = await _context.Fixture
                .OrderBy(f => f.Date)
                .ThenBy(f => f.Team.AgeGroup)
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Referee)
                .Where(f => (f.Date >= StartDateFilter && f.Date <= EndDateFilter))
                .Where(f => f.IsHome == IncludeHomeFilter || f.IsHome != IncludeAwayFilter)
                .ToListAsync();

            var q = _context.Fixture
                .OrderBy(f => f.Date)
                .ThenBy(f => f.Team.AgeGroup)
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Referee)
                .Where(f => (f.Date >= StartDateFilter && f.Date <= EndDateFilter))
                .Where(f => f.IsHome == IncludeHomeFilter || f.IsHome != IncludeAwayFilter)
                    .ToQueryString();

            //List
            //Fixture.OrderBy(f => f.Date)
            //    .ThenBy(f => f.Team.AgeGroup);
            //Fixture.OrderBy
        }
    }
}
