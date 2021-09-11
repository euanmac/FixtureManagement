using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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
                string shortName = Name.Replace("Thame United", ""); 
                return $"{GroupDescription} - {shortName}";
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
        JPL

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
        One=1, Two=2, Three=3, Four=4, Five=5, Six=6,  Red=20, White=21, Blue=22, Black=23, Green=24, Yellow=25, West = 70, Other =100
    }
}