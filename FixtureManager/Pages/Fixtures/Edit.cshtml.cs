using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.Fixtures
{

    [Authorize]
    public class EditModel : PageModel
    {

        private readonly FixtureManager.Data.ApplicationDBContext _context;
        public EditModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Fixture Fixture { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, Guid? teamId)
        {
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
           ViewData["TeamId"] = new SelectList(_context.Team, "Id", "DisplayName");

           return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync([FromQuery(Name = "teamId")] Guid? teamId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Fixture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixtureExists(Fixture.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (teamId == null)
            {
                return RedirectToPage("./Index");
            } else
            {
                return RedirectToPage("/Teams/Details", new { id = teamId });
            }

        }

        private bool FixtureExists(Guid id)
        {
            return _context.Fixture.Any(e => e.Id == id);
        }
    }
}
