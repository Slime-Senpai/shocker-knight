using System;
using System.Collections.Generic;

namespace ShockerKnight;

public class PiShockWsOperation(
    int hubId,
    int shockerId,
    int userId,
    string name,
    int op,
    double duration,
    int intensity)
{
    public PiShockWsOperation(PiShockConfiguration.PiShockSettings config, Operations.BaseOperation operation, string nameExtra)
        : this(config.HubId, config.ShockerId, config.UserId, config.Name + nameExtra, operation.Op, operation.Duration, operation.Intensity)
    {
    }

    public string Operation { get; set; } = "PUBLISH";
    public List<PiShockWsPublishCommand> PublishCommands { get; set; } = [new(hubId, shockerId, userId, name, op, duration, intensity)];

    public class PiShockWsPublishCommand
    {
        public PiShockWsPublishCommand(int hubId, int shockerId, int userId, string name, int op, double duration, int intensity)
        {
            Target = $"c{hubId}-ops";
            var mode = op switch
            {
                0 => "s",
                2 => "b",
                _ => "v"
            };
            Body = new PiShockWsPublishBody(shockerId, userId, name, mode, duration, intensity);
        }

        public string Target { get; set; }
        public PiShockWsPublishBody Body { get; set; }
    }

    public class PiShockWsPublishBody
    {
        public PiShockWsPublishBody(int shockerId, int userId, string name, string mode, double duration, int intensity)
        {
            id = shockerId;
            m = mode;
            r = true;
            i = intensity;
            d = (int)Math.Round(duration * 1000);
            l = new PiShockWsL(userId, name);
        }


        private int _intensity;
        private int _duration;
        public int id { get; set; }
        public string m { get; set; }
        public bool r { get; set; }
        public PiShockWsL l { get; set; }

        public int i
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

        public int d
        {
            get => _duration;
            set
            {
                _duration = value switch
                {
                    < 300 => 300,
                    > 15000 => 15000,
                    _ => value
                };
            }
        }
    }

    public class PiShockWsL(int userId, string name)
    {
        public int u { get; set; } = userId;
        public string ty { get; set; } = "api";
        public bool w { get; set; } = false;
        public string o { get; set; } = name;
    }
}