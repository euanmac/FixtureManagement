using System;
namespace FixtureManager.Models.ViewModels
{
    public class FixtureAllocationDetail
    {
        private Fixture _fixture;
        private Team _team;
        private FixtureAllocation _allocation;

        public FixtureAllocationDetail(Team team, Fixture fixture, FixtureAllocation allocation)
        {
            _fixture = fixture;
            _team = team;
            _allocation = allocation;
        }

        public string FixtureDescription
        {
            get
            {
                return $"{_team.DisplayName} v {_fixture.Opponent} on {_fixture.Date.ToShortDateString()}";
            }
        }

    }
}
