using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.Bookings
{
    public class DetailsModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public DetailsModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public Booking Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Booking = await _context.Booking
                .Include(b => b.Pitch)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Booking == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
