using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HorseRace.Controllers
{
    [Route("api/[controller]")]
    public class PresenceController
    {
        private readonly RawEndPoint _rawEndPoint;

        public PresenceController(RawEndPoint rawEndPoint)
        {
            _rawEndPoint = rawEndPoint;
        }

        [HttpGet]
        public IEnumerable<(string, string)> GetRawConnections()
        {
            return _rawEndPoint.Connections.Select(context =>
                (context.ConnectionId,
                context.Metadata.TryGetValue("LastSent", out var lastSent) ? (string)lastSent : "<empty>"));
        }
    }
}
