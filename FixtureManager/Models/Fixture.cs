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
        [Display(Name = "F")]
        Friendly,
        [Display(Name = "L")]
        League,
        [Display(Name = "C")]
        Cup,
        Other
    }

}
