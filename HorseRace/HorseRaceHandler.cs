using System;
using HorseRace.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HorseRace
{
    public class HorseRaceHandler : IHorseRaceHandler
    {
        private readonly IHubContext<RaceHub> _raceHubContext;

        public HorseRaceHandler(IHubContext<RaceHub> raceHubContext)
        {
            _raceHubContext = raceHubContext;
        }

        public void StartingRace(Horse[] entrants, TimeSpan timeToStart)
        {
        }

        public void UpdatePositions(Horse[] horsePositions)
        {
            _raceHubContext.Clients.All.InvokeAsync("UpdatePositions", new[] { horsePositions });
        }

        public void RaceCanceled()
        {
        }

        public void RaceCompleted(Horse[] finalPositions, TimeSpan timeToNextRace)
        {
        }
    }
}
