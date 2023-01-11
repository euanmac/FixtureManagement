using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace FixtureManager.Models
{
    public class TeamReconiliationRow
    {
        public Team Team { get; set; }
        public Guid Id { get; set; }
        public FixtureRecMatchType RecStatus { get; set; }
        [DataType(DataType.Date)]
        public DateOnly MatchDate { get; set; }
        public bool IsAllocated { get; set; }
        public Guid AllocationId { get; set; }
        public bool CanAllocate { get; set; }
        public string Venue { get; set; }
        public string Opponent { get; set; }
        public string Pitch { get; set; }
        [DataType(DataType.Time)]
        public TimeOnly Start { get; set; }
        public string FullTimeURL { get; set; }
        public FixtureType FixtureType { get; set; }
    }

    public enum FixtureRecMatchType
    {
        unknownTeam,
        [Display(Name = "No Fixture")]
        noFixture,
        [Display(Name = "Fixture ok, matches not FullTime")]
        localFixtureOnly,
        [Display(Name = "Fixture not matched on FullTime")]
        localFixtureUnmatched,
        [Display(Name = "Only on FullTime, needs to be downloaded")]
        fullTimeUnmatched,
        [Display(Name = "Matched on FullTime")]
        fullTimematched,
    }
}
