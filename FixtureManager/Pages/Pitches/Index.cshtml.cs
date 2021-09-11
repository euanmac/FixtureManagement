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

    public class RecurringEvent: Event
    {
        public List<int> DaysOfWeek { get; set; } = new List<int>();
        public string StartTime
        {
            get
            {
                return (DaysOfWeek.Count() > 0) ? Start.ToString("HH:mm") : "";
            }
        }

        public string EndTime
        {
            get
            {
                return (DaysOfWeek.Count() > 0) ? End.ToString("HH:mm") : "";
            }
        }
    }

    public class IndexModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;

        public List<Event> Recurring
        {
            get
            {
                return new List<Event>
                {
                    new RecurringEvent { Id = Guid.NewGuid(), Title= "U7, U10, U12 Training",Start=DateTime.Parse("2021-08-29T08:45"), End = DateTime.Parse("2022-05-30T10:15"), Color="black", ResourceId=Guid.Parse("9E365BFE-DDC2-4296-83DF-B278A846207F") , DaysOfWeek = {6 } },
                    new RecurringEvent { Id = Guid.NewGuid(), Title= "U8, U9 Training",Start=DateTime.Parse("2021-08-29T10:30"), End = DateTime.Parse("2022-05-30T11:45"), Color="black", ResourceId=Guid.Parse("9E365BFE-DDC2-4296-83DF-B278A846207F") , DaysOfWeek = {6 } },
                    new RecurringEvent { Id = Guid.NewGuid(), Title= "U8, U9 Training",Start=DateTime.Parse("2021-08-29T10:30"), End = DateTime.Parse("2022-05-30T11:45"), Color="black", ResourceId=Guid.Parse("FF4B08C8-DC9A-4F4A-9EC1-BADA343145B9") , DaysOfWeek = {6 } }

                };


            }
        }

        public IndexModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Pitch> Pitch { get; set; }
        public List<Event> Event { get; set; }
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
            Event.AddRange(Recurring);

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            ResourceJSON = JsonSerializer.Serialize(Pitch, serializeOptions);
            //Have to cast to object to ensure inherted type (recurring events) are serialised properly
            EventJSON = JsonSerializer.Serialize(Event.Cast<object>().ToList(), serializeOptions);
        }

        
    }
}
