using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.Fixtures
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;
        public Team ForTeam { get; set; }

        public CreateModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["TeamIdList"] = new SelectList(_context.Team, "Id", "DisplayName");

            //Validate that any team parameter is a valid team id
            string teamId = Request.Query["teamId"].FirstOrDefault();
            Guid teamGuid = Guid.Empty;
            if (Guid.TryParse(teamId, out teamGuid)) {
                ForTeam = _context.Team.First(t => t.Id == teamGuid);

                //Get max date if there is one and add 7 days. Otherwise default to today
                DateTime maxDate = _context.Fixture
                     .Where(f => f.TeamId == teamGuid)
                     .Select(f => f.Date.AddDays(7))
                     .Max(f => (DateTime?)f.Date) ?? DateTime.Now.Date;
                
              Fixture = new Fixture { TeamId = ForTeam.Id, Date= maxDate, FixtureType=FixtureType.League };
            }
            
            return Page();
        }



        [BindProperty]
        public Fixture Fixture { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync([FromQuery(Name = "teamId")] Guid? teamId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            _context.Fixture.Add(Fixture);
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
