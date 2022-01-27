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
         
       
        //Navigation properties
        [Ignore]
        public Team Team { get; set; }
        [Ignore]
        public FixtureAllocation FixtureAllocation { get; set; }



    }

    public class DownloadFixture
    {
 
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Home")]
        public bool IsHome { get; set; }
        public string Opponent { get; set; }
        [Display(Name = "Type")]
        public FixtureType FixtureType { get; set; }
        public bool Add { get; set; }

    }

    public enum FixtureType
    {
        Friendly,
        League,
        Cup,
        Cancelled,
        Postponed,
        Other
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
                _ => "O"
            };
        }
    }
}


