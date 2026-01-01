using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShockerKnight.Operations;

namespace ShockerKnight;

public class PiShockAPI(PiShockConfiguration.PiShockSettings config)
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://do.pishock.com/api/apioperate/")
    };

    public async Task<string> SendShockAsync(int duration, int intensity, string nameExtra = "")
    {
        return await SendOperationAsync(new ShockOperation(duration, intensity), nameExtra);
    }

    public async Task<string> SendVibrationAsync(int duration, int intensity, string nameExtra = "")
    {
        return await SendOperationAsync(new VibrationOperation(duration, intensity), nameExtra);
    }

    public async Task<string> SendBeepAsync(int duration, string nameExtra = "")
    {
        return await SendOperationAsync(new BeepOperation(duration), nameExtra);
    }

    private async Task<string> SendOperationAsync(BaseOperation operation, string nameExtra = "")
    {
        var json = JsonConvert.SerializeObject(new PiShockOperation(config, operation, nameExtra));
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("", content);
        // If it's not a success code, throw an exception for simplicity
        response.EnsureSuccessStatusCode();
        // Otherwise, return the response cause why not
        return await response.Content.ReadAsStringAsync();
    }

    private class PiShockOperation(
        string username,
        string apikey,
        string code,
        string name,
        int op,
        int duration,
        int intensity)
    {
        public PiShockOperation(PiShockConfiguration.PiShockSettings config, BaseOperation operation, string nameExtra)
            : this(config.Username, config.Apikey, config.Code, config.Name + nameExtra, operation.Op, operation.Duration, operation.Intensity)
        {
        }

        public string Username { get; set; } = username;
        public string Apikey { get; set; } = apikey;
        public string Code { get; set; } = code;
        public string Name { get; set; } = name;
        public int Op { get; set; } = op;

        private int _duration = duration;

        public int Duration
        {
            get => _duration;
            set
            {
                _duration = value switch
                {
                    < 0 => 0,
                    > 15 => 15,
                    _ => value
                };
            }
        }

        private int _intensity = intensity;

        public int Intensity
        {
            get => _intensity;
            set
            {
                {
                    _intensity = value switch
                    {
                        < 0 => 0,
                        > 100 => 100,
                        _ => value
                    };
                }
            }
        }
    }
}