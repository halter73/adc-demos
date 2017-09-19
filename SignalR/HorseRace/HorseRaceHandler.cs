using HorseRace.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;

namespace HorseRace
{
    public class HorseRaceHandler : IHorseRaceHandler
    {
        private readonly IHubContext<RaceHub> _raceHub;

        public HorseRaceHandler(IHubContext<RaceHub> raceHub)
        {
            _raceHub = raceHub;
        }

        public void StartingRace(Horse[] entrants, TimeSpan timeToStart)
        {
        }

        public void UpdatePositions(Horse[] horsePositions)
        {
            _raceHub.Clients.All.InvokeAsync("UpdatePositions", new[] { horsePositions });
        }

        public void RaceCanceled()
        {
        }

        public void RaceCompleted(Horse[] finalPositions, TimeSpan timeToNextRace)
        {
        }
    }
}
