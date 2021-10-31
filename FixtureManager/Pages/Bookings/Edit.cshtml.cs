using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.Bookings
{
    public class EditModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public EditModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; }


        //public IEnumerable<SelectListItem> PitchList
        //{
        //    get
        //    {
        //        return _context.Pitch
        //                .Select(f => new SelectListItem()
        //                {
        //                    Text = f.Name,
        //                    Value = f.Id.ToString()
        //                });
        //    }
        //}

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

            ViewData["PitchId"] = new SelectList(_context.Pitch, "Id", "Name");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(Booking.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BookingExists(Guid id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
