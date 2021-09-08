using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Data;
using FixtureManager.Models;

namespace FixtureManager.Controllers
{

    public class FixtureResponse
    {
        public Guid Id { get; set; }
        public bool IsHome { get; set; }
        public string Opponent { get; set; }
        public FixtureType FixtureType { get; set; }
        public Guid TeamId { get; set; }
        public string Team { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public Gender Gender { get; set; }
        public League League { get; set; }
        public Division Division { get; set; }
        public Guid FixtureAllocationId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int Duration { get; set; }
        public Guid? RefereeId { get; set; }
        public string RefereeName { get; set; }
        public Guid PitchId { get; set; }
        public string PitchName { get; set; }

    }


    [Route("api/[controller]")]
    [ApiController]
    public class FixtureController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public FixtureController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Fixture
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FixtureResponse>>> GetFixture()
        {
            return await _context.Fixture
                .Include(f => f.Team)
                .Include(f => f.FixtureAllocation)
                    .ThenInclude(fa => fa.Pitch)
                .Include(f => f.FixtureAllocation)
                .Where(f => f.FixtureAllocation != null && f.FixtureAllocation.Pitch != null)

                .Select(f => new FixtureResponse
                {
                    Id = f.Id,
                    IsHome = f.IsHome,
                    Opponent = f.Opponent,
                    FixtureType = f.FixtureType,
                    TeamId = f.TeamId,
                    Team = f.Team.Name,
                    AgeGroup = f.Team.AgeGroup,
                    Gender = f.Team.Gender,
                    League = f.Team.League,
                    Division = f.Team.Division,
                    FixtureAllocationId = f.FixtureAllocation.Id,
                    Start = f.FixtureAllocation.Start.ToString(),
                    End = f.FixtureAllocation.End.ToString(),
                    Duration = f.FixtureAllocation.Duration.Minutes,
                    //RefereeId = f.FixtureAllocation.RefereeId,
                    PitchId = f.FixtureAllocation.Pitch.Id,
                    PitchName = f.FixtureAllocation.Pitch.Name
                })
                .ToListAsync();
        }

        // GET: api/Fixture/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fixture>> GetFixture(Guid id)
        {
            var fixture = await _context.Fixture.FindAsync(id);

            if (fixture == null)
            {
                return NotFound();
            }

            return fixture;
        }

        // PUT: api/Fixture/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFixture(Guid id, Fixture fixture)
        {
            if (id != fixture.Id)
            {
                return BadRequest();
            }

            _context.Entry(fixture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixtureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Fixture
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Fixture>> PostFixture(Fixture fixture)
        {
            _context.Fixture.Add(fixture);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFixture", new { id = fixture.Id }, fixture);
        }

        // DELETE: api/Fixture/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFixture(Guid id)
        {
            var fixture = await _context.Fixture.FindAsync(id);
            if (fixture == null)
            {
                return NotFound();
            }

            _context.Fixture.Remove(fixture);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FixtureExists(Guid id)
        {
            return _context.Fixture.Any(e => e.Id == id);
        }
    }
}
