using System;

namespace HorseRace
{
    public class HorseRaceHandler : IHorseRaceHandler
    {
        public void StartingRace(Horse[] entrants, TimeSpan timeToStart)
        {
        }

        public void UpdatePositions(Horse[] horsePositions)
        {
        }

        public void RaceCanceled()
        {
        }

        public void RaceCompleted(Horse[] finalPositions, TimeSpan timeToNextRace)
        {
        }
    }
}
