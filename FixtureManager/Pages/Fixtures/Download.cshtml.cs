using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using HtmlAgilityPack;
using FixtureManager.Models;


namespace FixtureManager.Pages.Fixtures
{
    [Authorize]
    public class DownloadModel : PageModel
    {
        private readonly FixtureManager.Data.ApplicationDBContext _context;
        public Team Team { get; set; }

        [BindProperty]
        public IList<DownloadFixture> DownloadFixtures { get; set; }

        public DownloadModel(FixtureManager.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Team = await _context.Team.
                Include(t => t.Fixtures)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (Team == null)
            {
                return NotFound();
            }

            DownloadFixtures = FixtureDownloader.FromFullTime(Team);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromQuery(Name = "Id")] Guid? teamId)
        {
            if (teamId == null)
            {
                return NotFound();
            }

            Team = await _context.Team.
                Include(t => t.Fixtures)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (Team == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (DownloadFixture df in DownloadFixtures)
            {
                if (df.Add)
                {
                    Fixture f = new Fixture { TeamId = Team.Id, Date = df.Date, FixtureType = df.FixtureType, Opponent = df.Opponent, IsHome = df.IsHome, Team = Team };
                    _context.Fixture.Add(f);
                }
            }
            _context.SaveChanges();

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }

    }


}

//var url = $"https://fulltime.thefa.com/fixtures.html?selectedSeason=156623977&selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam=93723802&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100";
//https://fulltime.thefa.com/fixtures.html?selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam=93723802&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100


//983642463
//https://fulltime.thefa.com/fixtures.html?selectedSeason=983642463&selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam=93723802&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100


//https://fulltime.thefa.com/fixtures.html?selectedSeason=983642463&selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam=93723802&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100
//https://fulltime.thefa.com/fixtures.html?selectedSeason=&selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam=93723802&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100
//https://fulltime.thefa.com/fixtures.html?selectedSeason=&selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam=93723802&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100