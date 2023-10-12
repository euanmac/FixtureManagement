using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Models;

namespace FixtureManager.Pages.Fixtures
{
    [Authorize]
    public class CreateBulkModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;
        public Team ForTeam { get; set; }

        public CreateBulkModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["TeamIdList"] = new SelectList(_context.Team, "Id", "DisplayName");

            //Validate that any team parameter is a valid team id
            string teamId = Request.Query["teamId"].FirstOrDefault();

            //Create array of dates from first Sunday in September to end of May
            DateOnly start = new DateOnly(DateTime.Now.Year, 9, 1);
            DateOnly firstSun = (start.DayOfWeek == 0) ? start : start.AddDays(7 - (int)start.DayOfWeek);
            DateOnly end = new DateOnly(DateTime.Now.Year + 1, 5, 31);
            //DateOnly end = new DateOnly(DateTime.Now.Year , 9, 14);
            TimeSpan interval = end.ToDateTime(TimeOnly.MinValue) - firstSun.ToDateTime(TimeOnly.MinValue);

            var dates = Enumerable.Range(0, interval.Days + 1)
                .Where(number => number % 7 == 0)
                .Select(d => firstSun.AddDays(d));

            Guid teamGuid = Guid.Empty;
            if (Guid.TryParse(teamId, out teamGuid)) {
                ForTeam = _context.Team
                    .Include(t => t.Fixtures)
                    .First(t => t.Id == teamGuid);

                //Get max date if there is one and add 7 days. Otherwise default to today
                DateTime maxDate = _context.Fixture
                     .Where(f => f.TeamId == teamGuid)
                     .Select(f => f.Date.AddDays(7))
                     .Max(f => (DateTime?)f.Date) ?? DateTime.Now.Date;

                Fixtures = dates.Select(d =>
                    {
                        Fixture existing = ForTeam.Fixtures.Find(f => f.Date == d.ToDateTime(TimeOnly.MinValue));
                        if (existing == null) {
                            return new Fixture { Id = Guid.NewGuid(), TeamId = ForTeam.Id, Date = d.ToDateTime(TimeOnly.MinValue), FixtureType = FixtureType.League };
                        } else {
                            return existing;
                        }
                    })
                    .ToList();
            }
            
            return Page();
        }



        [BindProperty]
        public IList<Fixture> Fixtures { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync([FromQuery(Name = "teamId")] Guid? teamId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (Fixture fixture in Fixtures)
            {
                if (fixture.Opponent != null)
                {
                    //Check if exists and update if so. Otherwise create new
                    if (_context.Fixture.Any(f => f.Id == fixture.Id)) {
                        _context.Attach(fixture).State = EntityState.Modified;

                    } else {
                        _context.Fixture.Add(fixture);
                    }
                }
            }
           
            await _context.SaveChangesAsync();

            if (teamId == null)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                return RedirectToPage("/Teams/Details", new { id = teamId });
            }

        }
    }
}
