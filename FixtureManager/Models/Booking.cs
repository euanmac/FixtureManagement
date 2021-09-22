using System;
using System.ComponentModel.DataAnnotations;

namespace FixtureManager.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public string Title { get; set;}
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid ResourceId { get; set; }
        public bool IsRecurring { get; set; }

    }
}
