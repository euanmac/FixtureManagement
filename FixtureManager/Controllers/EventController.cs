using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Pages.Pitches;
using FixtureManager.Models;

//git remote set-url --add https://euanmac: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c@github.com/euanmac/FixtureManagement.git
//git remote add origin https://euanmac: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c@github.com/euanmac/FixtureManagement.git

namespace FixtureManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EventController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Event
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Object>>> GetEvent(Guid? eventId, Guid? resourceId)
        {
            List<Event> events = await _context.Fixture
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                .Where(f => (f.FixtureAllocation != null && f.FixtureAllocation.Pitch != null) 
                        && (f.FixtureAllocation.IsApproved || User.Identity.IsAuthenticated))
                .Select(f => new Event(f, User.Identity.IsAuthenticated))
                .ToListAsync();

            List<Event> eventBookingSingle = _context.Booking
                .Where(b => !b.IsRecurring)
                .Select(b => new Event(b))
                .ToList();


            List<RecurringEvent> eventBookingRecurring = _context.Booking
                .Where(b => b.IsRecurring)
                .Select(b => new RecurringEvent(b))
                .ToList();

            events.AddRange(eventBookingRecurring);
            events.AddRange(eventBookingSingle);

            if (eventId != null)
            {
                events = events.Where(e => e.Id == eventId).ToList();
            }

            return events.Cast<object>().ToList();
        }


        private async Task<List<Event>> GetEventList(Guid? eventId, Guid? resourceId)
        {
            //Get fixtures
            IQueryable<Fixture> eventFixture = _context.Fixture
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                .Where(f => (f.FixtureAllocation != null && f.FixtureAllocation.Pitch != null) 
                        && (f.FixtureAllocation.IsApproved || User.Identity.IsAuthenticated));
                        
            if (eventId != null)
            {
                eventFixture = eventFixture.Where(f => f.Id == eventId);
            }
            if (resourceId != null)
            {
                eventFixture = eventFixture.Where(e => e.FixtureAllocation.PitchId == resourceId);
            }

            List<Event> events = await eventFixture.Select(f => new Event(f, User.Identity.IsAuthenticated))
                .ToListAsync();

            //Get single event bookings
            IQueryable<Booking> eventBookingSingle = _context.Booking
                .Where(b => !b.IsRecurring);

            if (eventId != null)
            {
                eventBookingSingle = eventBookingSingle.Where(b => b.Id == eventId);
            }
            if (resourceId != null)
            {
                eventBookingSingle = eventBookingSingle.Where(b => b.PitchId == resourceId);
            }
            events.AddRange(await eventBookingSingle
                .Select(b => new RecurringEvent(b))
                .ToListAsync());


            //Get recurring bookings
            IQueryable<Booking> eventBookingRecurring = _context.Booking
                .Where(b => b.IsRecurring);

            if (eventId != null)
            {
                eventBookingRecurring = eventBookingRecurring.Where(b => b.Id == eventId);
            }
            if (resourceId != null)
            {
                eventBookingRecurring = eventBookingRecurring.Where(b => b.PitchId == resourceId);
            }
            events.AddRange(await eventBookingRecurring
                .Select(b => new RecurringEvent(b))
                .ToListAsync());


            return events;

        }

        // GET: api/Event/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Object>>> GetEvents(Guid id)
        {
            IList<Event> events = await GetEventList(id, null);
            return events.Cast<object>().ToList();
   
        }


        // GET: api/Event/Resource/5
        [HttpGet("Resource/{resourceid:Guid}")]
        public async Task<ActionResult<IEnumerable<Object>>> GetEventsByResource(Guid resourceid)
        {
            IList<Event> events = await GetEventList(null, resourceid);
            return events.Cast<object>().ToList();

        }

        //[HttpGet("Resource/{id:Guid}")]
        //public async Task<ActionResult<Event>> GetEventByResource(Guid resourceid)
        //{
        //    var fixture = await _context.Fixture.
        //        Include((f => f.FixtureAllocation))
        //        .FirstAsync(f => f.Id == resourceid);

        //    if (fixture == null)
        //    {
        //        return NotFound();
        //    }

        //    return new Event(fixture);
        //}



        // PUT: api/Event/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(Guid id, Event @event)
        {
            if (id != @event.Id)
            {
                return BadRequest();
            }
     
            try
            {
                FixtureAllocation allocation = await _context.FixtureAlloctation
                    .FirstAsync(fa => fa.FixtureId == id);

                allocation.Start = @event.Start.ToLocalTime();
                allocation.Duration = @event.End - @event.Start;
                allocation.PitchId = @event.ResourceId;
                _context.Attach(allocation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return NoContent();
        }

        // POST: api/Event
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Event>> PostEvent(Event @event)
        //{
        //    //_context.Event.Add(@event);
        //    //await _context.SaveChangesAsync();

        //    //return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        //}

        //// DELETE: api/Event/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteEvent(Guid id)
        //{
        //    //var @event = await _context.Event.FindAsync(id);
        //    //if (@event == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //_context.Event.Remove(@event);
        //    //await _context.SaveChangesAsync();

        //    //return NoContent();
        //}

        private bool EventExists(Guid id)
        {
            return _context.FixtureAlloctation.Any(fa => fa.FixtureId == id);
        }


    }
}



//git remote add set-url https://euanmac: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@https://github.com/euanmac/FixtureManagement.git

//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