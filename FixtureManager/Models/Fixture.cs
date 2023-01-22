using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace FixtureManager.Models
{
    public class Fixture
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Home")]
        public bool IsHome { get; set; }
        public string Opponent { get; set; }
        [Display(Name = "Type")]
        public FixtureType FixtureType { get; set; }

        public bool CanAllocate
        {
            get
            {
                return IsHome && (FixtureType != FixtureType.Postponed && FixtureType != FixtureType.Cancelled);
            }
        }

        public bool IsAllocated
        {
            get
            {
                return (FixtureAllocation != null && FixtureAllocation.Pitch != null
                    && FixtureAllocation.Pitch.Name != null
                    && (FixtureAllocation.Start.Minute != 0
                    || FixtureAllocation.Start.Hour != 0));
            }
        }

        //Navigation properties
        [Ignore]
        public Team Team { get; set; }
        [Ignore]
        public FixtureAllocation FixtureAllocation { get; set; }
    }


    public enum FixtureType
    {
        Friendly =0,
        League =1,
        Cup =2,
        [Display(Name = "County Cup")]
        CountyCup =6,
        Cancelled =3,
        Postponed =4,
        Other =5
    }

    // Define an extension method in a non-nested static class.
    public static class Extensions
    {
        public static string ShortFixtureName(this FixtureType fixtureType)
        {
            return fixtureType switch
            {
                FixtureType.Friendly => "F",
                FixtureType.League => "L",
                FixtureType.Cup => "C",
                FixtureType.Postponed => "P",
                FixtureType.Cancelled => "X",
                FixtureType.CountyCup => "CC",
                _ => "O"
            };
        }

        public static FixtureType FromFullTimeFixtureType(this League league, string FTType)
        {
            return FTType switch
            {
                "F" => FixtureType.Friendly,
                "Cup" => FixtureType.Cup,
                "C" => FixtureType.CountyCup,
                "L" => FixtureType.League,
                "ONE" => FixtureType.League,
                "TWO" => FixtureType.League,
                "HL2N" => FixtureType.League,
                _ => FixtureType.Other
            };

        }
    }



}


