using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShockerKnight.Operations;

namespace ShockerKnight;

public class PiShockHttpAPI(PiShockConfiguration.PiShockSettings config, Log log) : PiShockAPI
{
    private HttpClient _client;

    public override Task<string> Initialize()
    {
        log("Initializing PiShock Http Client");
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://do.pishock.com/api/apioperate/")
        };

        return base.Initialize();
    }

    public override Task<string> Dispose()
    {
        log("Disposing PiShock Http Client");
        _client.Dispose();

        return base.Dispose();
    }

    protected override async Task<string> SendOperationAsync(BaseOperation operation, string nameExtra = "")
    {
        var json = JsonConvert.SerializeObject(new PiShockHttpOperation(config, operation, nameExtra));
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        log("Sending punishment via HTTP");

        var response = await _client.PostAsync("", content);
        // If it's not a success code, throw an exception for simplicity
        response.EnsureSuccessStatusCode();

        // Otherwise, return the response cause why not
        var responseBody = await response.Content.ReadAsStringAsync();

        log($"Response: {responseBody}");

        return responseBody;
    }
}