using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Pages.Bookings
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
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; }


        public IEnumerable<SelectListItem> PitchList
        {
            get
            {
                return _context.Pitch
                        .Select(f => new SelectListItem()
                        {
                            Text = f.Name,
                            Value = f.Id.ToString()
                        });
            }
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Booking.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
