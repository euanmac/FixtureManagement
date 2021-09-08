using System;
using System.Linq;
using FixtureManager.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Expressions;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace FixtureManager.Data
{
    public class DBInitialiser
    {
        public static void Initialize(ApplicationDBContext context)
        {
            // Look for any teams.
            if (context.Team.Any())
            {
                return;   // DB has been seeded
            }

            var people = new Person[]
            {
                new Person{ FirstName="Euan", LastName="Macfarlane", Email="euan2@mac.com", IsRef=false, Tel="07595001778"},
                new Person{ FirstName="Ben", LastName="Ward", Email="benjamin.j.ward@gmail.com", IsRef=false, Tel="07590 446162"},
                new Person{ FirstName="Sam", LastName="Smith", Email="sam_smith92@hotmail.co.uk", IsRef=true, Tel="07515 404658"},
                new Person{ FirstName="Sean", LastName="Miller", Email="sm@jystcreative.com", IsRef=true, Tel="07784 697541"},
                new Person{ FirstName="Ashley", LastName="Pearce", Email="ash.pearce@hotmail.co.uk", IsRef=false, Tel="07425 173189"},
                new Person{ FirstName="Rob", LastName="Phillips", Email="rob.phillips@boxtechnologies.com", IsRef=false, Tel="07747 603392"},
                new Person{ FirstName="Steve", LastName="Miller", Email="stevenmiller353@btinternet.com", IsRef=true, Tel="07425 173189"}


            };

            context.Person.AddRange(people);
            context.SaveChanges();

            var teams = new Team[]
            {
                new Team{ Name="Thame United Boys", AgeGroup=AgeGroup.U14, Gender = Gender.Male, League=League.WycAndSouthBucksMinor, Id = Guid.NewGuid()},
                new Team{ Name="Thame United Youth", AgeGroup=AgeGroup.U14, Gender = Gender.Male, League=League.WycAndSouthBucksMinor, Id = Guid.NewGuid()},
                new Team{ Name="Thame United", AgeGroup=AgeGroup.U17, Gender = Gender.Male, Id = Guid.NewGuid()}

            };

            context.Team.AddRange(teams);
            context.SaveChanges();

            var pitches = new Pitch[]
            {
                new Pitch{ Name = "Pitch 1" },
                new Pitch{ Name = "Pitch 2" },
                new Pitch{ Name = "Pitch 3" },
                new Pitch{ Name = "Pitch 4"},
                new Pitch{ Name = "Large 3G"},
                new Pitch{ Name = "Small Astro"},
                new Pitch{ Name = "Meadow"}
            };

            context.Pitch.AddRange(pitches);
            context.SaveChanges();


            var contacts = new TeamContact[]
            {
                new TeamContact{ Team=teams[0], TeamId=teams[0].Id, Person=people[0], PersonId=people[0].Id, ContactType = ContactType.Coach},
                new TeamContact{ Team=teams[0], TeamId=teams[0].Id, Person=people[1], PersonId=people[1].Id, ContactType = ContactType.Manager},
                new TeamContact{ Team=teams[2], TeamId=teams[2].Id, Person=people[5], PersonId=people[5].Id, ContactType = ContactType.Manager},
                new TeamContact{ Team=teams[2], TeamId=teams[2].Id, Person=people[6], PersonId=people[6].Id, ContactType = ContactType.Coach}

            };
            context.TeamContact.AddRange(contacts);
            context.SaveChanges();

            var fixtures = new Fixture[]
            {
                new Fixture{ Opponent="Long Crendon", TeamId=teams[0].Id, IsHome=true, Date=DateTime.Parse("04-Aug-2021"), Team = teams[0], FixtureType = FixtureType.League},
                new Fixture{ Opponent="Burnham Jaguars", TeamId=teams[0].Id, IsHome=true, Date=DateTime.Parse("11-Aug-2021"), Team = teams[0], FixtureType = FixtureType.Friendly}

            };

            context.Fixture.AddRange(fixtures);
            context.SaveChanges();


            var fixture = fixtures[0];
            var pitch = pitches[0];

            var allocations = new FixtureAllocation[]
            {
                new FixtureAllocation{ FixtureId=fixture.Id, Fixture=fixture, PitchId=pitch.Id, Pitch=pitch, Duration=new TimeSpan(0,90,0)}
            };

            context.FixtureAlloctation.AddRange(allocations);
            context.SaveChanges();
        }


        public static void InitializeFromCSV(ApplicationDBContext context)
        {

            // Look for any teams.
            if (context.Team.Any())
            {
                return;   // DB has been seeded
            }

            //People
            context.Person.AddRange(GetCSVData<Person>("FixtureManager.Data.Seed.Person.csv"));
            context.SaveChanges();

            //Teams
            context.Team.AddRange(GetCSVData<Team>("FixtureManager.Data.Seed.Team.csv"));
            context.SaveChanges();


            //Team Pitches
            context.Pitch.AddRange(GetCSVData<Pitch>("FixtureManager.Data.Seed.Pitch.csv"));
            context.SaveChanges();

            //Team Fixtures
            context.Fixture.AddRange(GetCSVData<Fixture>("FixtureManager.Data.Seed.Fixture.csv"));
            context.SaveChanges();
            //var f = GetCSVData<Fixture>("FixtureManager.Data.Seed.Fixture.csv");

            //Team Contacts
            context.TeamContact.AddRange(GetCSVData<TeamContact>("FixtureManager.Data.Seed.TeamContact.csv"));
            context.SaveChanges();

            //FIxture Allocation 
            context.FixtureAlloctation.AddRange(GetCSVData<FixtureAllocation>("FixtureManager.Data.Seed.FixtureAlloctation.csv"));
            context.SaveChanges();

        }

        private static List<T> GetCSVData<T>(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {

                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    return csvReader.GetRecords<T>().ToList();

                }
            }
        }
    }
}