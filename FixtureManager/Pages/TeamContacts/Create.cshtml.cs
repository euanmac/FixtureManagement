using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.TeamContacts
{
    public class CreateModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public CreateModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["PersonId"] = new SelectList(_context.Person.OrderBy(p => p.LastName), "Id", "FullName");
        ViewData["TeamId"] = new SelectList(_context.Team.OrderBy(t => t.AgeGroup), "Id", "DisplayName");
            return Page();
        }

        [BindProperty]
        public TeamContact TeamContact { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TeamContact.Add(TeamContact);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
