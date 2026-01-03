using System;
using System.Threading.Tasks;

namespace ShockerKnight;

public class PiShockHandler(PiShockConfiguration config, Log log)
{
    private PiShockAPI _api;

    private readonly Random _random = new();

    /// <summary>
    ///     Initializes the PiShockHandler by initializing the underlying API.
    /// </summary>
    public async Task<string> Initialize()
    {
        try
        {
            if (_api != null && _api.IsInitialized())
            {
                await _api.Dispose();
            }

            _api = config.PiShockSecrets.ConnectMode switch
            {
                PiShockConfiguration.ConnectMode.Http => new PiShockHttpAPI(config.PiShockSecrets, log),
                PiShockConfiguration.ConnectMode.Ws => new PiShockWsAPI(config.PiShockSecrets, log),
                _ => throw new ArgumentOutOfRangeException()
            };
            return await _api.Initialize();
        }
        catch (Exception)
        {
            // Make sure the process doesn't crash
            return string.Empty;
        }
    }

    /// <summary>
    ///     Releases resources used by the PiShockHandler and the underlying API.
    /// </summary>
    public async Task<string> Dispose()
    {
        try
        {
            if (_api == null) return string.Empty;

            return await _api.Dispose();
        }
        catch (Exception)
        {
            // Make sure the process doesn't crash
            return string.Empty;
        }
    }

    /// <summary>
    ///     Restarts the PiShockHandler by disposing of and reinitializing the underlying API.
    /// </summary>
    public async void Restart()
    {
        try
        {
            await Dispose();

            await Initialize();
        }
        catch (Exception)
        {
            // Make sure the process doesn't crash
        }
    }

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
        if (!settings.Enabled)
        {
            return;
        }

        var minDuration = Math.Min(settings.MinDuration, settings.MaxDuration);
        var maxDuration = Math.Max(settings.MinDuration, settings.MaxDuration);

        var duration = (minDuration + _random.NextDouble() * (maxDuration - minDuration)) * durationMultiplier;

        var minIntensity = Math.Min(settings.MinIntensity, settings.MaxIntensity);
        var maxIntensity = Math.Max(settings.MinIntensity, settings.MaxIntensity);

        var intensity = (int)Math.Round(_random.Next(minIntensity, maxIntensity + 1) * intensityMultiplier);

        if (!settings.Overcharge)
        {
            duration = Math.Min(Math.Max(duration, minDuration), maxDuration);
            intensity = Math.Min(Math.Max(intensity, minIntensity), maxIntensity);
        }

        switch (settings.Mode)
        {
            case PiShockConfiguration.Mode.Shock:
                _ = _api.SendShockAsync(duration, intensity, $"-{settings.Name}");
                break;
            default:
            case PiShockConfiguration.Mode.Vibration:
                _ = _api.SendVibrationAsync(duration, intensity, $"-{settings.Name}");
                break;
            case PiShockConfiguration.Mode.Beep:
                _ = _api.SendBeepAsync(duration, $"-{settings.Name}");
                break;
        }
    }
}