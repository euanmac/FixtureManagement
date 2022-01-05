using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FixtureManager.Models;
using FixtureManager.Data;
using System.Text.Json;

namespace FixtureManager.Pages.Pitches
{
    public class IndexModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;


        public IndexModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Pitch> Pitch { get; set; }
        public List<Event> Event { get; set; }
        public string ResourceJSON { get; set;}
        public string EventJSON { get; set; }
        public string InitialView { get; set; }
        public string EventURL { get; set; }
        public string Title { get; set; }
        public string HeaderButtons { get; set; }
        public string InitialDate { get; set; }

        public async Task OnGetAsync(Guid? pitchId, DateTime? date)
        {
            Pitch = await _context.Pitch
                .Where(p => p.DisplayOrder >= 0)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            Event = await _context.Fixture
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                .Where(f => f.FixtureAllocation != null && f.FixtureAllocation.Pitch != null)
                .Select(f => new Event(f))  
                .ToListAsync();

            Event.AddRange(RecurringEvent.Recurring);

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            ResourceJSON = JsonSerializer.Serialize(Pitch, serializeOptions);
            //Have to cast to object to ensure inherted type (recurring events) are serialised properly
            EventJSON = JsonSerializer.Serialize(Event.Cast<object>().ToList(), serializeOptions);

            bool showDayView = (pitchId == null);

            EventURL = showDayView ? "'/api/Event'" : $"'/api/Event/Resource/{pitchId}'";           
            InitialView = showDayView ? "'resourceTimeGridDay'" : "'timeGridWeek'";
            Title = showDayView ? "Fixture Grid" : $"{Pitch.Where(p => p.Id == pitchId).FirstOrDefault().Name}";
            HeaderButtons = showDayView ? "'today prevSat,prev,next,nextSat'" : "'today prev,next'";

            InitialDate = $"'{(date ?? DateTime.Now).ToString("yyyy-MM-dd")}'";
        }

        
    }
}
