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

namespace FixtureManager.Pages.FixtureAllocations
{
    public class EditModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public EditModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string SendingURL { get; set; }

        [BindProperty]
        public FixtureAllocation FixtureAllocation { get; set; }

        [BindProperty]
        public int DurationMins { get; set; }

        //public IEnumerable<SelectListItem> DurationList
        //{
        //    get
        //    {
        //        return new SelectListItem[] {
        //            new SelectListItem("60", "60"),
        //            new SelectListItem("75", "75"),
        //            new SelectListItem("90", "90"),
        //            new SelectListItem("105", "105"),
        //            new SelectListItem("120", "120")
        //        }.ToList();
        //    }
        //}

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FixtureAllocation = await _context.FixtureAlloctation
                .Include(f => f.Fixture)
                .Include(f => f.Pitch)
                .Include(f => f.Referee).FirstOrDefaultAsync(m => m.Id == id);

            if (FixtureAllocation == null)
            {
                return NotFound();
            }

            SendingURL = Request.Headers["Referer"].ToString();
            ViewData["PitchId"] = new SelectList(_context.Pitch, "Id", "Name");

            ViewData["RefereeId"] = new SelectList(_context.Person.Where(p => p.IsRef), "Id", "FullName");

            ViewData["FixtureId"] = _context.Fixture
                .Include(f => f.Team)
                .ToList()
                .Select(f => new SelectListItem()
                {
                    Text = $"{f.Team.DisplayName} v {f.Opponent} on {f.Date.ToShortDateString()}",
                    Value = f.Id.ToString()
                });

            DurationMins = Convert.ToInt32(FixtureAllocation.Duration.TotalMinutes);

            ViewData["Durations"] = new SelectListItem[] {
                    new SelectListItem("60", "60"),
                    new SelectListItem("75", "75"),
                    new SelectListItem("90", "90"),
                    new SelectListItem("105", "105"),
                    new SelectListItem("120", "120")
                }.ToList();
            
        
           return Page();


        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fixture = _context.Fixture
            .Where(f => f.Id == FixtureAllocation.FixtureId)
            .AsNoTracking()
            .First<Fixture>();

            if (fixture == null)
            {
                return Page();
            }

            FixtureAllocation.Duration = new TimeSpan(0, DurationMins, 0);
            FixtureAllocation.Start = fixture.Date.Add(FixtureAllocation.Start.TimeOfDay);
            _context.Attach(FixtureAllocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixtureAllocationExists(FixtureAllocation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Fixtures/Index");
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            _context.Remove(FixtureAllocation);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixtureAllocationExists(FixtureAllocation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Fixtures/Index");
        }

        private bool FixtureAllocationExists(Guid id)
        {
            return _context.FixtureAlloctation.Any(e => e.Id == id);
        }

    }


}
