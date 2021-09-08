using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HtmlAgilityPack;
using FixtureManager.Models;

namespace FixtureManager.Pages.Fixtures
{
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

            DownloadFixtures = GetFixturesFromFullTime(Team);
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

        private IList<DownloadFixture> GetFixturesFromFullTime(Team team)

        {

            List<(Fixture, bool)> fixtureList = new List<(Fixture,bool)>();
            List<DownloadFixture> dFixtures = new List<DownloadFixture>();

            //Get fixtures from FulLTime
            if (Team.FullTimeTeamId == null || Team.FullTimeLeagueId == null)
            {
                return dFixtures;
            }

            //var url = $"https://fulltime.thefa.com/fixtures.html?selectedSeason={Team.FullTimeLeagueId}&selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam={Team.FullTimeTeamId}&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100";
            var url = $"https://fulltime.thefa.com/displayTeam.html?divisionseason={Team.FullTimeLeagueId}&teamID={Team.FullTimeTeamId}";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            var rows = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr");
            int id = 0;

            try
            {
                foreach (var row in rows)
                {
                    string type = row.SelectSingleNode("./td[1]").InnerText.Trim();
                    string date = row.SelectSingleNode("./td[2]").InnerText.Trim().Substring(0, 8);
                    string home = row.SelectSingleNode("./td[3]").InnerText.Trim();
                    string away = row.SelectSingleNode("./td[7]").InnerText.Trim();
                    string link = row.SelectSingleNode("./td[3]/a").Attributes[0].Value;
                    int index = link.IndexOf("=") + 1;
                    string fixtureId = link.Substring(index, link.Length - index);

                    DateTime fdate = DateTime.Parse(date, new CultureInfo("en-GB"));
                    bool isHome = home.ToLower().IndexOf("thame ") >= 0;
                    string opponent = isHome ? away : home;

                    FixtureType ftype = type switch
                    {
                        "L" => FixtureType.League,
                        "Cup" => FixtureType.Cup,
                        "F" => FixtureType.Friendly,
                        _ => FixtureType.Other
                    };

                    var fixture = new Fixture { Date = fdate, IsHome = isHome, Opponent = opponent, TeamId = team.Id, Team = team, FixtureType = ftype, Id = Guid.NewGuid()};
                    var downloadFixture = new DownloadFixture { Id= id, Date = fdate, IsHome = isHome, Opponent = opponent, FixtureType = ftype, Add = true };
                    id++;

                    fixtureList.Add((fixture, true));
                    dFixtures.Add(downloadFixture);
                }

            }

            catch (Exception e)
            {
                Console.WriteLine($"Error getting fixtures for team {team.Id}");
            }

            return dFixtures;

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