using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.TeamContacts
{
    public class DetailsModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public DetailsModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public TeamContact TeamContact { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TeamContact = await _context.TeamContact
                .Include(t => t.Person)
                .Include(t => t.Team).FirstOrDefaultAsync(m => m.Id == id);

            if (TeamContact == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
