using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace FixtureManager.Models
{
    public class FixtureAllocation
    {
        public Guid Id { get; set; }
        public Guid FixtureId { get; set; }
        public Guid? PitchId { get; set; }
        public Guid? RefereeId { get; set; }
        [DataType(DataType.Time)]
        [Required]
        public DateTime Start { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:HH\\:mm}")]
        public TimeSpan Duration { get; set; }

        //Navigation Properties
        [Ignore]
        public Fixture Fixture { get; set; }
        [Ignore]
        public Pitch Pitch { get; set; }
        [Ignore]
        public Person Referee { get; set; }

        //Calculated Properties
        public bool IsComplete
        {
            get
            {
                return (RefereeId != null && PitchId != null && Duration != null);
            }
        }

        [DisplayFormat(DataFormatString = "{0:HH\\:mm}")]
        public DateTime End
        {
            get
            {
                return Start.Add(Duration);
            }

        }
    }
}
