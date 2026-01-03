namespace ShockerKnight;

public class PiShockConfiguration
{
    public enum Mode
    {
        Shock = 0,
        Vibration = 1,
        Beep = 2
    };

    public class BasePunishmentSettings(string name = "Punishment")
    {
        public bool Enabled = false;

        public Mode Mode = 0;

        public int MinIntensity = 0;

        public int MaxIntensity = 100;

        public double MinDuration = 0;

        public double MaxDuration = 15;

        public bool Overcharge = false;

        public readonly string Name = name;
    }

    public class DamagePunishmentSettings() : BasePunishmentSettings("Damage");

    public class DeathPunishmentSettings() : BasePunishmentSettings("Death");

    public DamagePunishmentSettings DamagePunishment { get; set; } = new();

    public DeathPunishmentSettings DeathPunishment { get; set; } = new();

    public PiShockSettings PiShockSecrets = new();

    public class PiShockSettings
    {
        public ConnectMode ConnectMode { get; set; }

        public string Username { get; set; }

        public string Apikey { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int HubId { get; set; }

        public int ShockerId { get; set; }

        public int UserId { get; set; }
    }

    public enum ConnectMode
    {
        Http,
        Ws
    }
}