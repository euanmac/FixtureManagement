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
        DateOnly MatchDay { get; set; }
        public DateOnly GameWeekStart { get; set; }
        public DateOnly GameWeekEnd { get; set; }
        public Dictionary<Team, List<TeamReconiliationRow>> FixtureRecResults { get; set; }

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

            //Summarise Fixtures by date, home and away, unallocated
            

          
        }

        struct SummaryRow
        {
            DateOnly SummaryDate { get; set; }
            int Home { get; set; }
            int Away { get; set; }
            int Allocated { get; set; }
            int CanMatch { get; set; }
            int Matched { get; set; }
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


        public string GetRecIcon(FixtureRecMatchType recType)
        {
            return recType switch
            {
                FixtureRecMatchType.noFixture => "bi bi-x-circle text-danger",
                FixtureRecMatchType.localFixtureOnly => "bi bi-journal-check text-success",
                FixtureRecMatchType.localFixtureUnmatched => "bi bi-journal-x text-danger",
                FixtureRecMatchType.fullTimeUnmatched => "bi bi-cloud-slash text-danger",               
                FixtureRecMatchType.fullTimematched => "bi bi-cloud-check text-success",                               
                _ => "bi bi-question-circle-fill text-danger    "
            };

        }
    }



}