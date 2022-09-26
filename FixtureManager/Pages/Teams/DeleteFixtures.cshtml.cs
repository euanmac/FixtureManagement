using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.Fixtures
{
    [Authorize]
    public class DeleteFixturesModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public DeleteFixturesModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Team = await _context.Team
                .Include(t => t.Fixtures)
                .FirstOrDefaultAsync(f => f.Id == id);
                
            if (Team == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Fixture> fixtures = await _context.Fixture
                    .Where(f => f.TeamId == id)
                    .ToListAsync();
            fixtures.ForEach(f =>
            {
                _context.Remove(f);
            });
            _context.SaveChanges();

            return RedirectToPage("./Index");
        }
    }
}
