using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HorseRace
{
    public class HorseRacer
    {
        private const int _fieldSize = 5;
        private const int _trackLength = 1000; // Not starting an argument about units
        private const int _minSpeed = 25; // units/_tickInterval (we can agree on seconds, right?)
        private const int _maxSpeed = 50;

        private static readonly string[] _firstNames = new[] { "Toledo", "Shreveport", "Dallas", "Carolina", "Virginia", "Peoria", "Italian", "Miami", "Jamaican", "Oregon", "French", "Irish", "Azerbaijani", "Kazakh", "King", "General", "Admiral", "Lieutenant", "Captain", "Major", "Commander", "Doctor", "Coach", "Señor ", "Señora", "Señorita", "Sir", "Senator", "Governor", "President", "Chief", "Prince", "Princess", "Queen", "Mr.", "Mrs.", "Ms.", "Fast", "Pretty", "Handsome", "Stunning", "Daring", "Sexy", "Smooth", "Distinguished", "Flattering", "Sassy", "Pink", "Sizzling", "Heavy", "Smart", "Brave", "Lucky", "Futuristic", "Merciful", "Ruthless", "Lethal", "Demonic", "Grizzly", "Horsey", "Dark", "Blue", "Purple", "Big", "Black", "White" };
        private static readonly string[] _lastNames = new[] { "Heat", "Lightning", "Rain", "Ice", "Thunder", "Wind", "Drizzle", "Air", "Winter", "Spring", "Autumn", "Summer", "Blizzard", "Breeze", "Flood", "Hail", "Mist", "Fog", "Gold", "Chrome", "Silver", "Diamond", "Platinum", "Bronze", "Pearl", "Ruby", "Sapphire", "Emerald", "Crystal", "Sky", "River", "Creek", "Lake", "Bog", "Sea", "Ocean", "Waves", "Forest", "Cat", "Dog", "Scorpion", "Lizard", "Panther", "Wildcat", "Tiger", "Lion", "Panda", "Sassy", "Daring", "Smooth", "Sexy", "LeBron", "Neymar", "Messi", "Jeter", "Joe", "Steve", "Ronaldo", "Sally", "McHorseFace", "Heisenberg", "Steel", "Whitewalker", "Lannister", "Stark", "Baratheon", "Tyrell", "Snow", "Bolton", "Martell", "Porzingod", "Keanu", "Ghost", "Streak", "Rose", "Mamba", "Diesel", "Wun Wun", "Fo&#39; shizzle" };
        private static readonly IEnumerable<string> _fullNames = GetHorseNames();
        private static readonly int[] _positionNumbers = Enumerable.Range(1, _fieldSize).ToArray();
        private static readonly TimeSpan _racePaddingTime = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan _tickInterval = TimeSpan.FromSeconds(1);

        private readonly IEnumerable<IHorseRaceHandler> _horseRaceHandlers;
        private readonly ILogger _logger;
        private HorsePosition[] _positions = new HorsePosition[0];

        public HorseRacer(IEnumerable<IHorseRaceHandler> horseRaceHandlers, ILoggerFactory loggerFactory)
        {
            _horseRaceHandlers = horseRaceHandlers;
            _logger = loggerFactory.CreateLogger<HorseRacer>();
        }

        public HorsePosition[] CurrentPositions => _positions;

        public async Task RunRaces(CancellationToken cancellationToken)
        {
            var random = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                var entrants = _fullNames.Take(_fieldSize).ToArray();
                _positions = _positionNumbers.Zip(entrants, (startingPos, name) =>
                {
                    return new HorsePosition
                    {
                        Position = startingPos,
                        Name = name,
                        Distance = 0,
                    };
                }).ToArray();

                _logger.LogWarning("Starting race with {entrants}. {Time} left to start.", string.Join(", ", entrants), _racePaddingTime);
                foreach (var handler in _horseRaceHandlers)
                {
                    handler.StartingRace(entrants, _racePaddingTime);
                }

                await Task.Delay(_racePaddingTime);

                while (_positions.Any(pos => pos.Distance < _trackLength) && !cancellationToken.IsCancellationRequested)
                {
                    var updatedDistances = _positions
                        .Select(pos => (pos.Name, pos.Distance == _trackLength ?
                                                    _trackLength + _maxSpeed : // Keep position of completed racers stable.
                                                    pos.Distance + random.Next(_minSpeed, _maxSpeed)))
                        .OrderByDescending(tuple => tuple.Item2);

                    _positions = _positionNumbers.Zip(updatedDistances, (pos, distanceTuple) =>
                    {
                        return new HorsePosition
                        {
                            Position = pos,
                            Name = distanceTuple.Item1,
                            Distance = Math.Min(distanceTuple.Item2, _trackLength),
                        };
                    }).ToArray();

                    foreach (var handler in _horseRaceHandlers)
                    {
                        handler.UpdatePositions(_positions);
                    }

                    await Task.Delay(_tickInterval);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    foreach (var handler in _horseRaceHandlers)
                    {
                        handler.RaceCanceled();
                    }
                    _logger.LogWarning("Cancellation requested. Quitting race.");
                    return;
                }

                _logger.LogWarning("Race completed! {entrant} is the winner!", _positions[0].Name);
                foreach (var handler in _horseRaceHandlers)
                {
                    handler.RaceCompleted(_racePaddingTime);
                }

                await Task.Delay(_racePaddingTime);
            }
        }

        private static IEnumerable<string> GetHorseNames()
        {
            var random = new Random();

            while (true)
            {
                yield return $"{_firstNames[random.Next(0, _firstNames.Length)]} {_lastNames[random.Next(0, _lastNames.Length)]}";
            }
        }
    }
}
