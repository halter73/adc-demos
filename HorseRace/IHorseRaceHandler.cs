using System;

namespace HorseRace
{
    public interface IHorseRaceHandler
    {
        void StartingRace(string[] entrants, TimeSpan timeToStart);
        void UpdatePositions(HorsePosition[] horsePositions);
        void RaceCompleted(TimeSpan timeToNextRace);
        void RaceCanceled();
    }
}
