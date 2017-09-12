using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorseRace.Hubs
{
    public class RaceHub : Hub
    {
        public async Task Bet(int id)
        {
            await Groups.AddAsync(Context.ConnectionId, id.ToString());
        }
    }
}
