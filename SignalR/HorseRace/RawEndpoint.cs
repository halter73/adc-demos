using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Sockets;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace HorseRace
{
    public class RawEndPoint : EndPoint, IHostedService
    {
        private readonly HorseRacer _horseRacer;

        public RawEndPoint(HorseRacer horseRacer)
        {
            _horseRacer = horseRacer;
        }

        public ConnectionList Connections { get; } = new ConnectionList();

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            Connections.Add(connection);

            await Broadcast($"{connection.ConnectionId} connected ({connection.Metadata[ConnectionMetadataNames.Transport]})");

            try
            {
                while (await connection.Transport.In.WaitToReadAsync())
                {
                    if (connection.Transport.In.TryRead(out var buffer))
                    {
                        var text = Encoding.UTF8.GetString(buffer);
                        connection.Metadata["LastSent"] = text;
                        text = $"{connection.ConnectionId}: {text}";
                        await Broadcast(Encoding.UTF8.GetBytes(text));
                    }
                }
            }
            finally
            {
                Connections.Remove(connection);

                await Broadcast($"{connection.ConnectionId} disconnected ({connection.Metadata[ConnectionMetadataNames.Transport]})");
            }
        }

        public async Task BroadcastPositionsAsync()
        {
            while (await _horseRacer.PositionReadableChannel.WaitToReadAsync())
            {
                if (_horseRacer.PositionReadableChannel.TryRead(out var horsePositions))
                {
                    // We can avoid the copy here but we'll deal with that later
                    var text = JsonConvert.SerializeObject(horsePositions);
                    text = $"Positions: {text}";
                    await Broadcast(text);
                }
            }
        }

        public Task StartAsync(CancellationToken token)
        {
            _ = BroadcastPositionsAsync();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        private Task Broadcast(string text)
        {
            return Broadcast(Encoding.UTF8.GetBytes(text));
        }

        private Task Broadcast(byte[] payload)
        {
            var tasks = new List<Task>(Connections.Count);

            foreach (var c in Connections)
            {
                tasks.Add(c.Transport.Out.WriteAsync(payload));
            }

            return Task.WhenAll(tasks);
        }
    }
}
