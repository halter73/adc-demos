using System;

namespace HorseRace
{
    public interface IHorseRaceHandler
    {
        void StartingRace(Horse[] entrants, TimeSpan timeToStart);
        void UpdatePositions(Horse[] horsePositions);
        void RaceCompleted(Horse[] finalPositions, TimeSpan timeToNextRace);
        void RaceCanceled();
    }
}
