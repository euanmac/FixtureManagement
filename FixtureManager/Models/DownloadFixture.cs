using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HtmlAgilityPack;
using System.Globalization;

namespace FixtureManager.Models
{
    public class FixtureDownloader
    {
        public static string FullTimeURL = "https://fulltime.thefa.com/displayTeam.html?divisionseason={0}&teamID={1}";
        public static IList<DownloadFixture> FromFullTime(Team team)
        {
            List<(Fixture, bool)> fixtureList = new List<(Fixture, bool)>();
            List<DownloadFixture> dFixtures = new List<DownloadFixture>();

            //Get fixtures from FulLTime
            if (team.FullTimeTeamId == null || team.FullTimeLeagueId == null)
            {
                return dFixtures;
            }

            //var url = $"https://fulltime.thefa.com/fixtures.html?selectedSeason={team.FullTimeLeagueId}&selectedFixtureGroupKey=&selectedDateCode=all&selectedClub=&selectedTeam={Team.FullTimeTeamId}&selectedRelatedFixtureOption=2&selectedFixtureDateStatus=&selectedFixtureStatus=&previousSelectedFixtureGroupAgeGroup=&previousSelectedFixtureGroupKey=&previousSelectedClub=&itemsPerPage=100";
            //var url = $"https://fulltime.thefa.com/displayTeam.html?divisionseason={team.FullTimeLeagueId}&teamID={team.FullTimeTeamId}";
            var url = String.Format(FullTimeURL, team.FullTimeLeagueId, team.FullTimeTeamId);
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
                    string home = HtmlEntity.DeEntitize(row.SelectSingleNode("./td[3]").InnerText.Trim());
                    string away = HtmlEntity.DeEntitize(row.SelectSingleNode("./td[7]").InnerText.Trim());
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

                    var fixture = new Fixture { Date = fdate, IsHome = isHome, Opponent = opponent, TeamId = team.Id, Team = team, FixtureType = ftype, Id = Guid.NewGuid() };
                    var downloadFixture = new DownloadFixture { Id = Guid.NewGuid(), Date = fdate, IsHome = isHome, Opponent = opponent, FixtureType = ftype, Add = true };
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

    public class DownloadFixture
    {
        public Guid Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Home")]
        public bool IsHome { get; set; }
        public string Opponent { get; set; }
        [Display(Name = "Type")]
        public FixtureType FixtureType { get; set; }
        public bool Add { get; set; }

    }
}
