using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace FixtureManager.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Age Group")]
        [Required]
        public AgeGroup AgeGroup { get; set; }
        public Gender Gender { get; set; }
        public League League { get; set; }
        public Division Division { get; set; }
        [Display(Name = "FullTime Team Id")]
        public String FullTimeTeamId { get; set; }
        [Display(Name = "FullTime League Id")]
        public String FullTimeLeagueId { get; set; }
        [Display(Name = "Club Ref Required")]
        public bool RefRequired { get; set; }


        [Display(Name = "Age Group")]
        public string GroupDescription
        {
            get
            { 
                switch (AgeGroup)
                {
                    case AgeGroup.Adult:
                        return (Gender == Gender.Mixed ? "" : (Gender == Gender.Male ? "Men" : "Women"));
                    case AgeGroup.U18:
                    case AgeGroup.External:
                    case AgeGroup.O50:

                        return $"{AgeGroup} " + (Gender == Gender.Mixed ? "" : (Gender == Gender.Male ? "Men" : "Women"));
                    default:
                        return $"{AgeGroup} " + (Gender == Gender.Mixed ? "" : (Gender == Gender.Male ? "Boys" : "Girls"));
                }
            }
        }


        [Display(Name = "Team Name")]
        public string DisplayName
        {
            get
            {
                return $"{GroupDescription} - {Name}";
            }
        }

        [Display(Name = "Team Name")]
        public string DisplayShortName
        {
            get
            {
                if (Name != null)
                {
                    string shortName = Name.Replace("Thame United", "");
                    return $"{GroupDescription} - {shortName}";
                }
                else
                {
                    return "";
                }
            }
        }


        [Display(Name = "Team Size")]
        public TeamSize TeamSize{
            get {
                switch (AgeGroup)
                {
                    case AgeGroup.U6:
                    case AgeGroup.U7:
                    case AgeGroup.U8:
                        return TeamSize._5v5;
                    case AgeGroup.U9:
                    case AgeGroup.U10:
                        return TeamSize._7v7;
                    case AgeGroup.U11:
                    case AgeGroup.U12:
                        return TeamSize._9v9;
                    default:
                        return TeamSize._11v11;
                }
           }
        }

        public string Colour
        {
            get
            {
                return League switch
                {
                    League.BucksGirls => "Crimson",
                    League.HighWycSunComb => "DimGray",
                    League.OxGirls => "Teal",
                    League.OxMailYouth => "Gold",
                    League.OxOver50 => "Silver",
                    League.SouthBucksMini => "OrangeRed",
                    League.TVCWFL => "MediumSeaGreen",
                    League.WycAndSouthBucksMinor => "SteelBlue",
                    League.JPL => "MidnightBlue"

                };
            }
        }

        public string FixtureURL
        {
            get
            {
                if (String.IsNullOrEmpty(FullTimeLeagueId) || String.IsNullOrEmpty(FullTimeTeamId))
                {
                    return "";
                }
                else
                {
                    return $"https://fulltime.thefa.com/displayTeam.html?divisionseason={FullTimeLeagueId}&teamID={FullTimeTeamId}";
                }

            }
        }

        //Navigation
        public List<Fixture> Fixtures { get; set; }
        public List<TeamContact> Contacts { get; set; }



        private IList<DownloadFixture> GetFixturesFromFullTime()

        {

            List<(Fixture, bool)> fixtureList = new List<(Fixture, bool)>();
            List<DownloadFixture> dFixtures = new List<DownloadFixture>();

            //Get fixtures from FulLTime
            if (FullTimeTeamId == null || FullTimeLeagueId == null)
            {
                return dFixtures;
            }

            var url = $"https://fulltime.thefa.com/displayTeam.html?divisionseason={FullTimeLeagueId}&teamID={FullTimeTeamId}";
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

                    var fixture = new Fixture { Date = fdate, IsHome = isHome, Opponent = opponent, TeamId = Id, Team = this, FixtureType = ftype, Id = Guid.NewGuid() };
                    var downloadFixture = new DownloadFixture { Id = id, Date = fdate, IsHome = isHome, Opponent = opponent, FixtureType = ftype, Add = true };
                    id++;

                    fixtureList.Add((fixture, true));
                    dFixtures.Add(downloadFixture);
                }

            }

            catch (Exception e)
            {
                Console.WriteLine($"Error getting fixtures for team {Id}");
            }

            return dFixtures;

        }

    }


    public enum AgeGroup
    {
        U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, U17, U18, O50, Adult, External
    }

    public enum League
    {
        [Display(Name = "South Bucks Mini Soccer Conference")]
        SouthBucksMini,
        [Display(Name = "Bucks Girls Football League")]
        BucksGirls,
        [Display(Name = "Wycombe and South Bucks Minor League")]
        WycAndSouthBucksMinor,
        [Display(Name = "Oxford Mail Youth Football League")]
        OxMailYouth,
        [Display(Name = "Oxfordshire Over 50s League")]
        OxOver50,
        [Display(Name = "High Wycombe Sunday Football Combination")]
        HighWycSunComb,
        [Display(Name = "Thames Valley Counties Women's Football League")]
        TVCWFL,
        [Display(Name = "Oxfordshire Girls Football League")]
        OxGirls,
        [Display(Name = "Junior Premier League")]
        JPL,
        [Display(Name = "Uhlsport Hellenic League")]
        UHL,
        [Display(Name = "Aylesbury and District Football League")]
        ADL
       
    }

    public enum TeamSize
    {
        [Display(Name = "5 v 5")]
        _5v5,
        [Display(Name = "7 v 7")]
        _7v7,
        [Display(Name = "9 v 9")]
        _9v9,
        [Display(Name = "11 v 11")]
        _11v11
    }

    public enum Gender
    {
       Male, Female, Mixed
    }
    public enum Division
    {
        One=1, Two=2, Three=3, Four=4, Five=5, Six=6,  Red=20, White=21, Blue=22, Black=23, Green=24, Yellow=25, West = 70, South = 71, North = 72, East = 73, Championship = 80, Premier = 81, Other =100
    }

    
}