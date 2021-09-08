using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FixtureManager.Models;
using FixtureManager.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FixtureManager.Pages.Pitches
{

    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid ResourceId { get; set; }
        public string Color { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public IndexModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IList<Pitch> Pitch { get; set; }
        public IList<Event> Event { get; set; }
        public string ResourceJSON { get; set;}
        public string EventJSON { get; set; }

        public async Task OnGetAsync()
        {
            Pitch = await _context.Pitch
                .Where(p => p.DisplayOrder > 0)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            Event = await _context.Fixture
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                .Where(f => f.FixtureAllocation != null && f.FixtureAllocation.Pitch != null)
                .Select(f => new Event
                {
                    Id = f.Id,
                    Title = $"{f.Team.DisplayName} v {f.Opponent}",
                    Start = f.FixtureAllocation.Start,
                    End = f.FixtureAllocation.End,
                    ResourceId = f.FixtureAllocation.PitchId.Value,
                    Color = f.Team.Colour

                })
                .ToListAsync();

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            ResourceJSON = JsonSerializer.Serialize(Pitch, serializeOptions);
            EventJSON = JsonSerializer.Serialize(Event, serializeOptions);
        }

        
    }
}
