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
        public async Task<ActionResult<IEnumerable<Object>>> GetEvent()
        {
            List<Event> events = await _context.Fixture
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                .Where(f => f.FixtureAllocation != null && f.FixtureAllocation.Pitch != null)
                .Select(f => new Event(f, User.Identity.IsAuthenticated))
                .ToListAsync();
            events.AddRange(RecurringEvent.Recurring);
            return events.Cast<object>().ToList();
        }

        // GET: api/Event/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(Guid id)
        {
            var fixture = await _context.Fixture.
                Include((f => f.FixtureAllocation))
                .FirstAsync(f => f.Id == id);

            if (fixture == null)
            {
                return NotFound();
            }

            return new Event(fixture);
        }

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
                allocation.Duration = @event.End.ToLocalTime() - @event.Start.ToLocalTime();
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
