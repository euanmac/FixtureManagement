using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace FixtureManager.Models
{
    public class TeamReconiliationRow
    {
        public Team Team { get; set; }
        public FixtureRecMatchType RecStatus { get; set; }
        [DataType(DataType.Date)]
        public DateOnly MatchDate { get; set; }
        public bool IsAllocated { get; set; }
        public string Venue { get; set; }
        public string Opponent { get; set; }
        public string Pitch { get; set; }
        [DataType(DataType.Time)]
        public DateTime Start { get; set; }
        public string FullTimeURL { get; set; }
        public FixtureType FixtureType { get; set; }
    }

    public enum FixtureRecMatchType
    {
        unknownTeam,
        noFixture,
        localFixtureOnly,
        localFixtureUnmatched,
        fullTimeUnmatched,
        fullTimematched,
    }
}
