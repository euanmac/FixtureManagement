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
    public class DeleteModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public DeleteModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Fixture Fixture { get; set; }
        [BindProperty]
        public string RefererURL { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, Guid? teamId)
        {
            RefererURL = Request.Headers.Referer;
            if (id == null)
            {
                return NotFound();
            }

            Fixture = await _context.Fixture
                .Include(f => f.Team).FirstOrDefaultAsync(m => m.Id == id);

            if (Fixture == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id, Guid? teamId)
        {
            if (id == null)
            {
                return NotFound();
            }

            Fixture = await _context.Fixture.FindAsync(id);

            if (Fixture != null)
            {
                _context.Fixture.Remove(Fixture);
                await _context.SaveChangesAsync();
            }

            if (RefererURL == null)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                //return RedirectToPage("/Teams/Details", new { id = teamId });
                return Redirect(RefererURL);
            }
        }
    }
}
