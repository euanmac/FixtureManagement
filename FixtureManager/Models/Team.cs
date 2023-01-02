using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using FixtureManager.Models.ViewModels;

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

        public IList<DownloadFixture> DownloadFixture { get; set; }


        public void RollAgeGroup()
        {
                AgeGroup++;
        }

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

        public bool IsOnFullTime
        {
            get
            {
                return ! ((FullTimeLeagueId is null || FullTimeTeamId is null)
                || (FullTimeTeamId.Length == 0 && FullTimeTeamId.Length == 0));
            }
        }

        public IList<TeamReconiliationRow> Reconcile(DateOnly start, DateOnly end)
        {

            //Download fixtures if team is on fulltime, filter so only within start and end date
            List<DownloadFixture> downloadFixtures = !this.IsOnFullTime ? new List<DownloadFixture>() :
                FixtureDownloader.FromFullTime(this) 
                .Where(f => (DateOnly.FromDateTime(f.Date) >= start && DateOnly.FromDateTime(f.Date) <= end))
                .ToList();

            //Get list of DB fixtures for date range
            List<Fixture> fixtures = this.Fixtures
                .Where(f => (DateOnly.FromDateTime(f.Date) >= start && DateOnly.FromDateTime(f.Date) <= end))
                .ToList();

            //Check for matching fixtures on FullTime
            var matchedRows = from f in fixtures
                              join df in downloadFixtures
                              on f.Date equals df.Date
                              where (f.Opponent == df.Opponent) && (f.IsHome == df.IsHome)
                              select new TeamReconiliationRow
                              {
                                  Team = this,
                                  MatchDate = DateOnly.FromDateTime(f.Date),
                                  RecStatus = FixtureRecMatchType.fullTimematched,
                                  Opponent = f.Opponent,
                                  FixtureType = f.FixtureType,
                                  Venue = f.IsHome ? "H" : "A"
                              };

            //If no local or fulltime fixtures then return rec row
            //Check for rows, if none, create empty row for team to represent no fixture
            if (downloadFixtures.Count == 0 && fixtures.Count == 0)
            {
                int offset = this.MatchDay == DayOfWeek.Sunday ? 6 : (int)this.MatchDay - 1;
                return new List<TeamReconiliationRow> { new TeamReconiliationRow { Team = this, MatchDate = start.AddDays(offset), RecStatus = FixtureRecMatchType.noFixture } };
            }

            //Find fixtures for dates
            //If fixtures exists then try to match
            IList<TeamReconiliationRow> recRows = fixtures.Select(f =>
                {
                    //Default row for local fixture
                    TeamReconiliationRow recRow = new TeamReconiliationRow {
                        Team = this,
                        MatchDate = DateOnly.FromDateTime(f.Date),
                        Opponent = f.Opponent,
                        Venue = (f.IsHome ? "H" : "A"),
                        FixtureType = f.FixtureType
 
                    };

                    //Check for downloaded fixture that matches
                    DownloadFixture downloaded = downloadFixtures.FirstOrDefault(df =>
                        (f.Date) == df.Date &&
                        f.Opponent == df.Opponent &&
                        f.IsHome == df.IsHome &&
                        f.FixtureType == df.FixtureType
                        );

                    //Found download so check whether it matches
                    if (downloaded != null)
                    {
                        recRow.RecStatus = FixtureRecMatchType.fullTimematched;
                        //Need to remove downloaded from list!
                        downloadFixtures.Remove(downloaded);   
                    }
                    //No match so just set whether there should be a full time match or not if team not set up 
                    else
                    {
                        recRow.RecStatus = this.IsOnFullTime ? FixtureRecMatchType.localFixtureUnmatched : FixtureRecMatchType.localFixtureOnly;
                    }
                    return recRow;
                 }).ToList();

            //Now need to find any downloaded fixtures that dont have a matching local fixture - any matching should already have been removed above
            IList<TeamReconiliationRow> downloadRecs = downloadFixtures
                .Select(df =>
                {
                    return new TeamReconiliationRow
                    {
                        Team = this,
                        MatchDate = DateOnly.FromDateTime(df.Date),
                        Opponent = df.Opponent,
                        Venue = (df.IsHome ? "H" : "A"),
                        RecStatus = FixtureRecMatchType.fullTimeUnmatched,
                        FixtureType = df.FixtureType

                    };
                }).ToList();

            return recRows.Concat(downloadRecs).ToList();

        }

        ////Return a list of fixtures for the team for a given gameweek
        //public IList<GameweekSummary> GetGameweekSummary(IList<Fixture> fixtures, DateOnly start, DateOnly end)
        //{

        //    IList<Fixture> matching = fixtures.Where(f => ((DateOnly.FromDateTime(f.Date)) >= start)
        //        && (DateOnly.FromDateTime(f.Date) <= end)
        //    ).ToList();

        //    //Sun(0) then -6
        //    //Sat(6) then 0
        //    //Mon(1) then -2    
        //    //Tue(2) then -3

        //    //Sun(0) then 0
        //    //Sat(6) then 1
        //    //Mon(1) then 6
        //    //Tue(2) then 5
        //    //WEd(3) then 4
        //    //Thur(4) then 3
        //    //Fri(5) then 2
        //    //Sat(6) then 1

        //    if (matching.Count == 0)
        //    {
        //        int offset = this.MatchDay == DayOfWeek.Sunday ? 6 : (int)this.MatchDay - 1;
        //        return new List<GameweekSummary> { new GameweekSummary { IsValidMatch = false, MatchDate = start.AddDays(offset), Team = this, OnFullTime = false, Opponent = "", Venue = "" } };

        //    }
        //    bool onFullTime = ((FullTimeLeagueId is null || FullTimeTeamId is null)
        //     || (FullTimeTeamId.Length == 0 && FullTimeTeamId.Length == 0));

        //    var url = String.Format(FixtureDownloader.FullTimeURL, FullTimeLeagueId, FullTimeTeamId);
            
        //    return matching.Select(m => {
                
        //        bool isAllocated = !(m.FixtureAllocation is null) && m.FixtureAllocation.IsComplete;
        //        string pitch = isAllocated ? m.FixtureAllocation.Pitch.Name : "";
        //        DateTime startTime = isAllocated ? m.FixtureAllocation.Start : DateTime.MinValue;
        //        FixtureRecMatchType rec = Reconcile(matching, start, end);


        //        return new GameweekSummary
        //            {
        //                IsValidMatch = true,
        //                IsAllocated = isAllocated,
        //                Team = this,
        //                MatchDate = DateOnly.FromDateTime(m.Date),
        //                Opponent = m.Opponent,
        //                Venue = m.IsHome ? "H" : "A",
        //                Pitch = pitch,
        //                Start = startTime,
        //                OnFullTime = onFullTime,
        //                FullTimeURL = url,
        //                RecStatus = rec
        //            };
        //        }
        //    ).ToList();

        //}



        //public FixtureRecMatchType Reconcile(IList<Fixture> fixtures, DateOnly start, DateOnly end)
        //{

        //    //Get next match date (i.e. Sat or Sun)
        //    //var date = forDate.Date.AddDays(1);
        //    //var days = ((int)MatchDay - (int)date.DayOfWeek + 7) % 7;
        //    //var matchDate = date.AddDays(days);


        //    //Get start and end of game week (Monday to Sunday)
        //    //DateOnly start = forDate.AddDays(-(int)forDate.DayOfWeek + 1);
        //    //DateOnly end = forDate.AddDays(-(int)forDate.DayOfWeek + 7);


        //    IList<Fixture> matching = fixtures.Where(f => ((DateOnly.FromDateTime(f.Date)) >= start)
        //        && (DateOnly.FromDateTime(f.Date) <= end)
        //    ).ToList();

        //    if (matching.Count == 0)
        //        return FixtureRecMatchType.noLocalFixture;

        //    if (matching.Count > 1)
        //        return FixtureRecMatchType.multipleFixture;

        //    //one fixture so now check whether it can be reconciled
        //    Fixture matchingFixture = matching[0];

        //    if ((matchingFixture.Team.FullTimeLeagueId is null || matchingFixture.Team.FullTimeTeamId is null)
        //        || (matchingFixture.Team.FullTimeTeamId.Length == 0 && matchingFixture.Team.FullTimeTeamId.Length == 0))
        //    {
        //        //Full time details not complete so cant reconciles
        //        return FixtureRecMatchType.notOnFullTime;
        //    }

        //    IList<DownloadFixture> downloaded = FixtureDownloader.FromFullTime(matchingFixture.Team);
        //    IList<DownloadFixture> matchingDownload = downloaded.Where(d => DateOnly.FromDateTime(d.Date) == DateOnly.FromDateTime(matchingFixture.Date)).ToList();

        //    //No matching fixture
        //    if (matchingDownload.Count == 0)
        //    {
        //        return FixtureRecMatchType.fullTimeNoFixture;
        //    }

        //    //No matching fixture
        //    if (matchingDownload.Count > 1)
        //    {
        //        return FixtureRecMatchType.fullTimeMultipleFixtures;
        //    }

        //    //Check location
        //    if (matchingDownload[0].IsHome != matchingFixture.IsHome)
        //    {
        //        return FixtureRecMatchType.fullTimeDifferentLocation;
        //    }

        //    if (matchingDownload[0].Opponent != matchingFixture.Opponent)
        //    {
        //        return FixtureRecMatchType.fullTimeDifferentOpponent;
        //    }

        //    //all match so return
        //    return FixtureRecMatchType.fullTimematched;

        //}



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
                    League.JPL => "MidnightBlue",
                    League.UHL => "DimGray",
                    League.ADL => "DimGray",
                    _ => "SteelBlue"
                };
            }
        }


        public DayOfWeek MatchDay
        {
            get
            {
                return League switch
                {
                    League.BucksGirls => DayOfWeek.Saturday,
                    League.HighWycSunComb => DayOfWeek.Sunday,
                    League.OxGirls => DayOfWeek.Saturday,
                    League.OxMailYouth => DayOfWeek.Sunday,
                    League.OxOver50 => DayOfWeek.Sunday,
                    League.SouthBucksMini => DayOfWeek.Sunday,
                    League.TVCWFL => DayOfWeek.Sunday,
                    League.WycAndSouthBucksMinor => DayOfWeek.Sunday,
                    League.JPL => DayOfWeek.Saturday,
                    League.UHL => DayOfWeek.Saturday,
                    League.ADL => DayOfWeek.Saturday,
                    _ => DayOfWeek.Sunday
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
        One=1, Two=2, Three=3, Four=4, Five=5, Six=6, Seven=7, Eight =8, Nine=9, Ten=10, Red=20, White=21, Blue=22, Black=23, Green=24, Yellow=25, West = 70, South = 71, North = 72, East = 73, Championship = 80, Premier = 81, Other =100
    }

    
}




/*



Rec 



*/