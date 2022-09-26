using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FixtureManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FixtureManager.Pages
{
    [Authorize]
    public class AdminModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        [BindProperty(SupportsGet = true)]
        public DateTime DeleteBefore { get; set; }

        public AdminModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
            DeleteBefore = DateTime.Now.Date;
        }

        public IList<Team> Team { get; set; }

        public void OnGetRoll()
        {
            _context.Team
                .Where(t => (t.AgeGroup <= AgeGroup.U17))
                .ToList()
                .ForEach(t => t.RollAgeGroup());
            _context.SaveChanges();

        }

        public void OnGetDeleteFixtures()
        {
            List<Fixture> fixtures = _context.Fixture
                .Where(f => f.Date < DeleteBefore)
                .ToList();
            _context.RemoveRange(fixtures);
            _context.SaveChanges();
      
            
        }
    }
}
