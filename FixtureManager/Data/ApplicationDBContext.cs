    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FixtureManager.Models;

namespace FixtureManager.Data
{

    public class ApplicationDBContext : DbContext
    {
        public DbSet<Team> Team { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Pitch> Pitch { get; set; }
        public DbSet<Fixture> Fixture { get; set; }
        //public DbSet<TUFixture> Fixture { get; set; }
        public DbSet<FixtureAllocation> FixtureAlloctation { get; set; }
        public DbSet<TeamContact> TeamContact { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Team Contact Configuration
            //builder.Entity<TeamContact>(
            //    tc =>
            //    {
            //        tc.HasKey(tc => new { tc.PersonId, tc.TeamId });
            //    });

            builder.Entity<FixtureAllocation>()
                .HasOne(fa => fa.Referee);

        }
    }
}
