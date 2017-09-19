using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorseRace.Hubs
{
    public class RaceHub : Hub
    {
        private readonly HorseRacer _horseRacer;

        public RaceHub(HorseRacer horseRacer)
        {
            _horseRacer = horseRacer;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId)
                .InvokeAsync("StartingRace", new[] { _horseRacer.CurrentPositions });
        }

        public async Task Bet(int id)
        {
            await Groups.AddAsync(Context.ConnectionId, id.ToString());
        }
    }
}
