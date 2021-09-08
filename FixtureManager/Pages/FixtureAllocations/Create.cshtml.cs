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
    public class CreateModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;
        private readonly Guid _fixtureId;

        public CreateModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> FixtureList
        {
            get
            {
                var list =_context.Fixture
                        .Include(f => f.FixtureAllocation)
                        .Include(f => f.Team)
                        .AsNoTracking()
                        .ToList()
                        .Where(f => (f.FixtureAllocation == null || !f.FixtureAllocation.IsComplete) && f.IsHome)
                        .Select(f => new SelectListItem()
                        {
                            Text = $"{f.Team.DisplayName} v {f.Opponent} on {f.Date.ToShortDateString()}",
                            Value = f.Id.ToString(),
                            Selected = FixtureAllocation == null ? false : FixtureAllocation.Fixture.Id == f.Id
                        });
                return list;
            }
        }

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

        public IEnumerable<SelectListItem> RefereeList
        {
            get
            {
                var list = _context.Person
                        .Where(p => p.IsRef)
                        .Select(f => new SelectListItem()
                        {
                            Text = f.FullName,
                            Value = f.Id.ToString()
                        });
               
                var blankList = new SelectListItem[] {
                    new SelectListItem()
                }.ToList();

                return blankList.Concat(list);

            }
        }

        public IEnumerable<SelectListItem> DurationList
        {
            get
            {
 
                return new SelectListItem[] {
                    new SelectListItem("60", "60"),
                    new SelectListItem("75", "75"),
                    new SelectListItem("90", "90"),
                    new SelectListItem("105", "105"),
                    new SelectListItem("120", "120")
                }.ToList();
            }
        }

        public async Task<IActionResult> OnGetAsync(Guid? fixtureId)
        {
            if (fixtureId != null)
            {

                var fixture = await _context.Fixture
                    .Include(f => f.Team)
                    .FirstOrDefaultAsync(f => f.Id == fixtureId);
                FixtureAllocation = new FixtureAllocation();
                {
                    FixtureAllocation.Fixture = fixture;
                    FixtureAllocation.FixtureId = fixture.Id;
                }

            }

            ViewData["fixtureId"] = _context.Fixture
                        .Include(f => f.FixtureAllocation)
                        .Include(f => f.Team)
                        .AsNoTracking()
                        .ToList()
                        .Where(f => (f.FixtureAllocation == null || !f.FixtureAllocation.IsComplete))
                        .Select(f => new SelectListItem()
                        {
                            Text = $"{f.Team.DisplayName} v {f.Opponent} on {f.Date.ToShortDateString()}",
                            Value = f.Id.ToString(),
                            Selected = FixtureAllocation == null ? false : FixtureAllocation.Fixture.Id == f.Id
                        });

            return Page();
        }

        [BindProperty]
        public int DurationMins { get; set; }

        [BindProperty]
        public FixtureAllocation FixtureAllocation { get; set; }

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

            _context.FixtureAlloctation.Add(FixtureAllocation);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Fixtures/Index");
        }
    }
}
