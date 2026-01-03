using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShockerKnight.Operations;

namespace ShockerKnight;

public class PiShockWsAPI(PiShockConfiguration.PiShockSettings config, Log log) : PiShockAPI
{
    private ClientWebSocket _socket;
    private HttpClient _client;

    public override async Task<string> Initialize()
    {
        try
        {
            log("Initializing PiShock WebSocket");
            _socket = new ClientWebSocket();

            _client = new HttpClient
            {
                BaseAddress = new Uri("https://auth.pishock.com/Auth/GetUserIfAPIKeyValid")
            };
            await _socket.ConnectAsync(new Uri($"wss://broker.pishock.com/v2?Username={config.Username}&ApiKey={config.Apikey}"), CancellationToken.None);

            if (config.UserId == 0)
            {
                var response = await _client.GetAsync($"?Username={config.Username}&ApiKey={config.Apikey}");

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var userId = JsonConvert.DeserializeObject<PiShockAuth>(responseBody).UserId;

                config.UserId = userId;

                log($"User ID: {userId}");
            }
        }
        catch (Exception e)
        {
            // We have to catch all exception to not crash the process
            log(e.Message);
        }

        var init = await base.Initialize();

        HandlePing();
        HandleReceive();

        return init;
    }

    private async void HandlePing()
    {
        while (IsInitialized() && _socket?.State == WebSocketState.Open)
        {
            try
            {
                var ping = "{\"Operation\":\"PING\"}";

                log($"Sending ping: {ping}");

                await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(ping)), WebSocketMessageType.Text, true, CancellationToken.None);

                await Task.Delay(40000);
            }
            catch (Exception e)
            {
                // We have to catch all exception to not crash the process
                log(e.Message);
            }
        }
    }

    private async void HandleReceive()
    {
        var buffer = new byte[1024 * 4];
        while (IsInitialized())
        {
            var segment = new ArraySegment<byte>(buffer);
            var t = await _socket.ReceiveAsync(segment, CancellationToken.None);

            log($"Received answer: {Encoding.UTF8.GetString(buffer, 0, t.Count)}");
        }
    }

    public override async Task<string> Dispose()
    {
        try
        {
            log("Disposing PiShock WebSocket");
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
            _socket.Dispose();
            _client.Dispose();
        }
        catch (Exception e)
        {
            // We have to catch all exception to not crash the process
            log(e.Message);
        }

        return await base.Dispose();
    }

    protected override async Task<string> SendOperationAsync(BaseOperation operation, string nameExtra = "")
    {
        var json = JsonConvert.SerializeObject(new PiShockWsOperation(config, operation, nameExtra));
        var bytes = Encoding.UTF8.GetBytes(json);

        log($"Sending punishment via WS: {json}");

        await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

        return "Done";
    }

    public class PiShockAuth
    {
        public int UserId { get; set; }
    }
}