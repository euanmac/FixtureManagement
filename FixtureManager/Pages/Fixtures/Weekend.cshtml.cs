using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Models;
using System.ComponentModel.DataAnnotations;


namespace FixtureManager.Pages.Fixtures
{
    public class FixtureSummary
    {
        public string Location;
        public bool OnFullTime;
        public bool FullTimeMatch;
    }

    public class TeamForecast
    {
        public Team Team { get; set; }
        public Dictionary<DateTime, FixtureSummary> FutureFixtures = new Dictionary<DateTime, FixtureSummary>();
    }

    public class WeekendModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public WeekendModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
            IncludeHomeFilter = true;
            IncludeAwayFilter = false;
            StartDateFilter = DateTime.Now.Date. ;
            EndDateFilter = DateTime.Now.Date.AddDays(21);
        }

        public IList<Fixture> Fixture { get;set; }
        public IList<Team> Team { get; set; }

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

            Fixture = _context.Fixture
               .OrderBy(f => f.Date)
               .ThenBy(f => f.Team.AgeGroup)
               .Include(f => f.Team)
               .Include(f => f.FixtureAllocation)
                   .ThenInclude(fa => fa.Pitch)
               .Include(f => f.FixtureAllocation)
               .Where(f => (f.Date >= StartDateFilter && f.Date <= EndDateFilter))
               .ToList();

            Team = await _context.Team
                .Include(t => t.Fixtures)
                .Where(t => t.Fixtures.Any(f => f.Date >= DateTime.Now.Date))
                .ToListAsync();




            IList<TeamForecast> fs = Team.Select(t =>
            {
                var groupedFixtures = t.Fixtures.GroupBy(f => f.Date);
                return new TeamForecast
                {
                    Team = t,
                    FutureFixtures = t.Fixtures
                    .ToDictionary(f => f.Date, f =>
                    {
                        return new FixtureSummary
                        {
                            Location = f.IsHome ? "H" : "A",
                            OnFullTime = false,
                            FullTimeMatch = false
                        };
                    })
                };
      
            }).ToList();



            DateTime nextWeekend = DateTime.Now;


            //List
            //Fixture.OrderBy(f => f.Date)
            //    .ThenBy(f => f.Team.AgeGroup);
            //Fixture.OrderBy
        }
    }
}

/*

 Team, Opponent     
 
 
 
 */
