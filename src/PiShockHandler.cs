using System;

namespace ShockerKnight;

public class PiShockHandler(PiShockConfiguration config)
{
    private readonly PiShockAPI _api = new(config.PiShockSecrets);

    private readonly Random _random = new();

    /// <summary>
    ///     Handles the damage taken event by sending a shock signal through the PiShock API.
    /// </summary>
    /// <param name="durationMultiplier">
    ///     A multiplier applied to the duration of the shock signal. Defaults to 1.
    /// </param>
    /// <param name="intensityMultiplier">
    ///     A multiplier applied to the intensity of the shock signal. Defaults to 1.
    /// </param>
    public void OnDamageTaken(float durationMultiplier = 1, float intensityMultiplier = 1)
    {
        HandlePunishment(durationMultiplier, intensityMultiplier, config.DamagePunishment);
    }

    /// <summary>
    ///     Handles the player death event by sending a shock signal through the PiShock API.
    /// </summary>
    /// <param name="durationMultiplier">
    ///     A multiplier applied to the duration of the shock signal. Defaults to 1.
    /// </param>
    /// <param name="intensityMultiplier">
    ///     A multiplier applied to the intensity of the shock signal. Defaults to 1.
    /// </param>
    public void OnPlayerDeath(float durationMultiplier = 1, float intensityMultiplier = 1)
    {
        HandlePunishment(durationMultiplier, intensityMultiplier, config.DeathPunishment);
    }

    /// <summary>
    ///     Processes and sends a signal to the PiShock API based on the supplied configuration settings.
    /// </summary>
    /// <param name="durationMultiplier">
    ///     A multiplier applied to the duration of the signal. Can go above the max if Overcharge is enabled.
    /// </param>
    /// <param name="intensityMultiplier">
    ///     A multiplier applied to the intensity of the signal. Can go above the max if Overcharge is enabled.
    /// </param>
    /// <param name="settings">
    ///     The configuration settings that dictate the behavior of the punishment, including mode, intensity range, and
    ///     duration range.
    /// </param>
    private void HandlePunishment(float durationMultiplier, float intensityMultiplier,
        PiShockConfiguration.BasePunishmentSettings settings)
    {
        var minDuration = Math.Min(settings.MinDuration, settings.MaxDuration);
        var maxDuration = Math.Max(settings.MinDuration, settings.MaxDuration);

        var duration = _random.Next(minDuration, maxDuration) * durationMultiplier;

        var minIntensity = Math.Min(settings.MinIntensity, settings.MaxIntensity);
        var maxIntensity = Math.Max(settings.MinIntensity, settings.MaxIntensity);

        var intensity = _random.Next(minIntensity, maxIntensity) * intensityMultiplier;

        if (!settings.Overcharge)
        {
            duration = Math.Min(Math.Max(duration, minDuration), maxDuration);
            intensity = Math.Min(Math.Max(intensity, minIntensity), maxIntensity);
        }

        switch (settings.Mode)
        {
            case PiShockConfiguration.Mode.Shock:
                _ = _api.SendShockAsync((int)Math.Round(duration), (int)Math.Round(intensity), $"-{settings.Name}");
                break;
            default:
            case PiShockConfiguration.Mode.Vibration:
                _ = _api.SendVibrationAsync((int)Math.Round(duration), (int)Math.Round(intensity), $"-{settings.Name}");
                break;
            case PiShockConfiguration.Mode.Beep:
                _ = _api.SendBeepAsync((int)Math.Round(duration), $"-{settings.Name}");
                break;
        }
    }
}