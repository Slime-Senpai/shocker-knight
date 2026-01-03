using System;

namespace ShockerKnight;

public class PiShockHttpOperation
{
    public PiShockHttpOperation(string username,
        string apikey,
        string code,
        string name,
        int op,
        double duration,
        int intensity)
    {
        Username = username;
        Apikey = apikey;
        Code = code;
        Name = name;
        Op = op;
        Duration = (int)Math.Round(duration);
        Intensity = intensity;
    }

    public PiShockHttpOperation(PiShockConfiguration.PiShockSettings config, Operations.BaseOperation operation, string nameExtra)
        : this(config.Username, config.Apikey, config.Code, config.Name + nameExtra, operation.Op, operation.Duration, operation.Intensity)
    {
    }

    public string Username { get; set; }
    public string Apikey { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Op { get; set; }

    private int _duration;

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

    private int _intensity;

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