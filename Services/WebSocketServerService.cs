using System.Text.Json;
using Fleck;
using CaseBattleBackend.Enums;
using static System.String;

namespace CaseBattleBackend.Services;

public class WebSocketServerService : IHostedService
{
    private WebSocketServer? _server;
    private readonly List<IWebSocketConnection> _allConnections = [];
    private readonly Dictionary<string, HashSet<IWebSocketConnection>> _subscriptions = [];

    private Timer? _keepAliveTimer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _server = new WebSocketServer("ws://0.0.0.0:8181/ws");

        _server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                Console.WriteLine("Client connected");
                _allConnections.Add(socket);
            };

            socket.OnClose = () =>
            {
                Console.WriteLine("Client disconnected");
                _allConnections.Remove(socket);
                foreach (var sub in _subscriptions.Values)
                    sub.Remove(socket);
            };

            socket.OnMessage = message =>
            {
                Console.WriteLine("Message received: " + message);

                try
                {
                    var doc = JsonDocument.Parse(message);
                    var type = doc.RootElement.GetProperty("type").GetString();
                    var channelStr = doc.RootElement.GetProperty("channel").GetString();

                    switch (type)
                    {
                        case "subscribe" when !IsNullOrWhiteSpace(channelStr):
                            if (!Enum.TryParse<SubscriptionChannel>(channelStr, ignoreCase: true, out var parsedChannel))
                            {
                                socket.Send(JsonSerializer.Serialize(new
                                {
                                    type = "error",
                                    message = $"Unknown channel: {channelStr}"
                                }));
                                return;
                            }

                            var channelKey = parsedChannel.ToString();

                            if (!_subscriptions.ContainsKey(channelKey))
                                _subscriptions[channelKey] = [];

                            _subscriptions[channelKey].Add(socket);

                            socket.Send(JsonSerializer.Serialize(new
                            {
                                type = "subscribed",
                                channel = channelKey
                            }));

                            Console.WriteLine($"Client subscribed to {channelKey}");
                            break;

                        case "unsubscribe" when !IsNullOrWhiteSpace(channelStr):
                            if (!Enum.TryParse<SubscriptionChannel>(channelStr, ignoreCase: true, out parsedChannel))
                            {
                                socket.Send(JsonSerializer.Serialize(new
                                {
                                    type = "error",
                                    message = $"Unknown channel: {channelStr}"
                                }));
                                return;
                            }

                            channelKey = parsedChannel.ToString();

                            if (_subscriptions.TryGetValue(channelKey, out var subscribers))
                            {
                                subscribers.Remove(socket);

                                socket.Send(JsonSerializer.Serialize(new
                                {
                                    type = "unsubscribed",
                                    channel = channelKey
                                }));

                                Console.WriteLine($"Client unsubscribed from {channelKey}");
                            }
                            else
                            {
                                socket.Send(JsonSerializer.Serialize(new
                                {
                                    type = "error",
                                    message = $"Not subscribed to channel: {channelKey}"
                                }));
                            }

                            break;

                        default:
                            socket.Send(JsonSerializer.Serialize(new
                            {
                                type = "error",
                                message = "Invalid or missing subscription request."
                            }));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    socket.Send(JsonSerializer.Serialize(new
                    {
                        type = "error",
                        message = "Invalid JSON: " + ex.Message
                    }));
                }
            };
        });

        Console.WriteLine("WebSocket server started on ws://0.0.0.0:8181");

        _keepAliveTimer = new Timer(KeepAlive, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _keepAliveTimer?.Dispose();

        foreach (var conn in _allConnections)
            conn.Close();

        return Task.CompletedTask;
    }

    public void PublishToChannel(SubscriptionChannel channel, object payload)
    {
        var channelKey = channel.ToString();

        if (!_subscriptions.TryGetValue(channelKey, out var subscribers))
            return;

        var json = JsonSerializer.Serialize(new
        {
            type = "message",
            channel = channelKey,
            data = payload
        });

        foreach (var conn in subscribers.ToList())
        {
            try
            {
                conn.Send(json);
            }
            catch
            {
                subscribers.Remove(conn);
            }
        }
    }

    public void Broadcast(object payload)
    {
        var json = JsonSerializer.Serialize(new
        {
            type = "broadcast",
            data = payload
        });

        foreach (var conn in _allConnections.ToList())
        {
            try
            {
                conn.Send(json);
            }
            catch
            {
                _allConnections.Remove(conn);
            }
        }
    }

    private void KeepAlive(object? state)
    {
        var json = JsonSerializer.Serialize(new
        {
            type = "Keep Alive",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        foreach (var conn in _allConnections.ToList())
        {
            try
            {
                if (conn.IsAvailable)
                    conn.Send(json);
            }
            catch
            {
                _allConnections.Remove(conn);
            }
        }
    }
}
