using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;
using FixtureManager.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace FixtureManager.Pages.Teams
{
    public class MatchdayModel : PageModel
    {

        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public MatchdayModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IList<Fixture> DownloadFixtures { get; set; }

        [BindProperty]
        DateOnly MatchDay { get; set; }

        [BindProperty]
        public IList<Guid> ConfirmAllocations {get; set;}

        public DateOnly GameWeekStart { get; set; }
        public DateOnly GameWeekEnd { get; set; }
        public Dictionary<Team, List<TeamReconiliationRow>> FixtureRecResults { get; set; }
        public int DownloadableCount { get
        { 
            return FixtureRecResults.Values.SelectMany(x => x).Where(x => x.RecStatus == FixtureRecMatchType.fullTimeUnmatched).Count(); }
        }

        public async Task OnGetAsync(DateTime? matchDayDate)
        {
            if (matchDayDate is null)
            {
                MatchDay = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                MatchDay = DateOnly.FromDateTime((DateTime)matchDayDate);
            }

            //Calculate Gameweek Start and end Dates
            int startOffset = (MatchDay.DayOfWeek == DayOfWeek.Sunday) ? 6 : (int)MatchDay.DayOfWeek - 1;
            int endOffset = (MatchDay.DayOfWeek == DayOfWeek.Sunday) ? 0 : 7 - (int)MatchDay.DayOfWeek;
            GameWeekStart = MatchDay.AddDays(-startOffset);
            GameWeekEnd = MatchDay.AddDays(endOffset);

            DateTime start = GameWeekStart.ToDateTime(TimeOnly.MinValue);
            DateTime end = GameWeekEnd.ToDateTime(TimeOnly.MinValue);

            //Fetch fixture details (and FullTime rows)
            IList<IList<TeamReconiliationRow>> rows = await _context.Team
               .Include(t => t.Fixtures)
                   .ThenInclude(f => f.FixtureAllocation)
                        .ThenInclude(fa => fa.Pitch)
               .Include(t => t.Contacts)
                   .ThenInclude(c => c.Person)
               .OrderBy(t => t.Gender)
                   .ThenBy(t => t.AgeGroup)
                   .ThenBy(t => t.Division)
               .Select(t => t.Reconcile( GameWeekStart, GameWeekEnd))
               .ToListAsync();

            //Flatten list and turn into dictionary keyed by team
            FixtureRecResults = rows
                .SelectMany(x => x)
                    .OrderBy(x => x.Team.AgeGroup)
                        .ThenBy(x => x.Team.Gender)
                        .ThenBy(x => x.Team.Division)
               .ToList()
               .GroupBy(r => r.Team).ToDictionary(g => g.Key, g => g.ToList());

        }

        public async Task<IActionResult> OnPostAsync(Guid teamId, string opponent, DateTime matchDate, string venue, FixtureType fixtureType, DateTime gameweekStart)
        {

            Team team = await _context.Team.
                Include(t => t.Fixtures)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                return NotFound();
            }

            Fixture f = new Fixture { TeamId = team.Id, Date = matchDate, FixtureType = fixtureType, Opponent = opponent, IsHome = (venue == "H"), Team = team };
            _context.Fixture.Add(f);

            _context.SaveChanges();
            return RedirectToPage("./Matchday",new { matchDayDate = gameweekStart.ToString("dd-MMM-yyyy")});
        }
        

        public async Task<IActionResult> OnPostDownloadAllAsync(DateTime gameweekStart)
        {
            foreach (Fixture f in DownloadFixtures) {
                //Fixture f = new Fixture { TeamId = df, Date = df.Date, FixtureType = df.FixtureType, Opponent = df.Opponent, IsHome = df.IsHome, Team = Team };
                _context.Fixture.Add(f);
                await _context.Entry(f)
                    .Reference(f => f.FixtureAllocation)
                    .LoadAsync();

            }
            //await _context.SaveChangesAsync();
            return RedirectToPage("./Matchday", new { matchDayDate = gameweekStart.ToString("dd-MMM-yyyy") });
        }

        public async Task<IActionResult> OnPostConfirmAllAsync(DateTime gameweekStart)
        {
            foreach (Guid faId in ConfirmAllocations) {
                //Fixture f = new Fixture { TeamId = df, Date = df.Date, FixtureType = df.FixtureType, Opponent = df.Opponent, IsHome = df.IsHome, Team = Team };
                FixtureAllocation fa = await _context.FixtureAlloctation.FirstAsync(fa => fa.Id == faId);
                if (fa != null) {
                    fa.IsApproved = true;
                    await _context.SaveChangesAsync();
                }

            }
            //await _context.SaveChangesAsync();
            return RedirectToPage("./Matchday", new { matchDayDate = gameweekStart.ToString("dd-MMM-yyyy") });
        }

    }



}
