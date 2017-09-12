using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace HorseRace.Hubs
{
    public class RaceHub : Hub
    {
        public async Task<string> SendMessage(string message)
        {
            await Clients.All.InvokeAsync("OnMessage", $"{Context.ConnectionId} {message}");
            return Context.ConnectionId;
        }
    }
}
