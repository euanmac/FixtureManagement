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
    public class FixtureFulltimeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public FixtureFulltimeController(ApplicationDBContext context)
        {
            _context = context;
        }

        //// GET: api/Event
        //[HttpGet]
        //public async Task<ActionResult<IList<TeamReconiliationRow>>> Reconcile(Guid teamId, DateTime matchDate)
        //{
        //    Team team = await _context.Team.Where(t => t.Id == teamId).FirstAsync();

        //    if (team is null)
        //    {
        //        return null;
        //    }

        //    IList<Fixture> fixtures = await _context.Fixture.Where(f => f.TeamId == team.Id).ToListAsync();

        //    //all match so return
        //    return team.Rec2(DateOnly.FromDateTime(matchDate), DateOnly.FromDateTime(matchDate));

        //}


    }
}



//git remote add set-url https://euanmac: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@https://github.com/euanmac/FixtureManagement.git

//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