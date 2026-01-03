using System;
using Modding;
using Satchel.BetterMenus;
using Satchel.BetterMenus.Config;
using InputField = UnityEngine.UI.InputField;

namespace ShockerKnight;

public class ShockerKnight() : Mod("ShockerKnight"), ICustomMenuMod, ITogglableMod, IGlobalSettings<PiShockConfiguration>
{
    private PiShockHandler _handler;
    private PiShockConfiguration _configuration;
    private Menu _menuRef;

    public override string GetVersion()
    {
        return "1.1.0";
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        _menuRef ??= PrepareMenu((ModToggleDelegates)toggleDelegates);

        return _menuRef.GetMenuScreen(modListMenu);
    }

    public bool ToggleButtonInsideMenu { get; } = true;

    public void OnLoadGlobal(PiShockConfiguration configuration)
    {
        _configuration = configuration;
        _configuration.PiShockSecrets.Name = "ShockerKnight";
    }

    public PiShockConfiguration OnSaveGlobal()
    {
        return _configuration;
    }

    public void Unload()
    {
        Log("Unloading");
        ModHooks.AfterTakeDamageHook -= AfterDamageTaken;
        ModHooks.AfterPlayerDeadHook -= OnPlayerDeath;
        _ = _handler.Dispose();
        Log("Unloaded successfully");
    }

    public override void Initialize()
    {
        Log("Initializing");
        _configuration ??= new PiShockConfiguration
        {
            DamagePunishment =
            {
                Enabled = false,
                MinIntensity = 1,
                MaxIntensity = 10,
                MinDuration = 1,
                MaxDuration = 3,
                Overcharge = false
            },
            DeathPunishment =
            {
                Enabled = false,
                MinIntensity = 10,
                MaxIntensity = 20,
                MinDuration = 1,
                MaxDuration = 3,
                Overcharge = false
            },
            PiShockSecrets =
            {
                ConnectMode = PiShockConfiguration.ConnectMode.Http,
                Name = "ShockerKnight"
            }
        };

        _handler = new PiShockHandler(_configuration, Log);
        _ = _handler.Initialize();

        ModHooks.AfterTakeDamageHook += AfterDamageTaken;
        ModHooks.AfterPlayerDeadHook += OnPlayerDeath;
        Log("Initialized");
    }

    private int AfterDamageTaken(int hazardType, int damage)
    {
        var durationMultiplier = PlayerData.instance.overcharmed ? damage * 2 : damage;

        Log($"Took {durationMultiplier} damage, going to shock now");

        var healthMultiplier =
            (float)(PlayerData.instance.maxHealthBase - PlayerData.instance.health -
                    PlayerData.instance.healthBlue) / PlayerData.instance.maxHealthCap;

        var multiplier = (float)Math.Max(0.5, 1 + healthMultiplier);

        _handler.OnDamageTaken(intensityMultiplier: multiplier, durationMultiplier: durationMultiplier);

        return damage;
    }

    private void OnPlayerDeath()
    {
        Log("Player died, going to shock now");

        _handler.OnPlayerDeath();
    }

    private Menu PrepareMenu(ModToggleDelegates toggleDelegates)
    {
        var inputFieldConfig = new InputFieldConfig
        {
            fontSize = 24,
            inputBoxWidth = 600f,
            contentType = InputField.ContentType.Standard,
            saveType = InputFieldSaveType.EditEnd
        };

        var apiMenuVisible = false;

        return new Menu("Shocker Knight", [
            toggleDelegates.CreateToggle("ShockerKnight Toggle", "Enables or disables the mod"),
            new TextPanel("PiShock Damage settings"),
            new HorizontalOption("Damage enabled", "Enables getting punished when taking damage", ["Off", "On"],
                setting => { _configuration.DamagePunishment.Enabled = setting == 1; },
                () => _configuration.DamagePunishment.Enabled ? 1 : 0),
            new HorizontalOption("Damage Mode", "Defines what should happen when you get damaged",
                ["Shock", "Vibration", "Beep"],
                setting =>
                {
                    _configuration.DamagePunishment.Mode = setting switch
                    {
                        0 => PiShockConfiguration.Mode.Shock,
                        2 => PiShockConfiguration.Mode.Beep,
                        _ => PiShockConfiguration.Mode.Vibration
                    };
                },
                () => (int)_configuration.DamagePunishment.Mode),
            new CustomSlider("Duration Min",
                value => _configuration.DamagePunishment.MinDuration = Math.Round(value, 1),
                () => (float)_configuration.DamagePunishment.MinDuration,
                0, 15),
            new CustomSlider("Duration Max",
                value => _configuration.DamagePunishment.MaxDuration = Math.Round(value, 1),
                () => (float)_configuration.DamagePunishment.MaxDuration,
                0, 15),
            new CustomSlider("Intensity Min",
                value => _configuration.DamagePunishment.MinIntensity = (int)value,
                () => _configuration.DamagePunishment.MinIntensity,
                0, 100, true),
            new CustomSlider("Intensity Max",
                value => _configuration.DamagePunishment.MaxIntensity = (int)value,
                () => _configuration.DamagePunishment.MaxIntensity,
                0, 100, true),
            new HorizontalOption("Overcharge", "Enables the punishment going above the max in certain cases",
                ["Off", "On"],
                setting => { _configuration.DamagePunishment.Overcharge = setting == 1; },
                () => _configuration.DamagePunishment.Overcharge ? 1 : 0),
            new TextPanel("PiShock Death settings"),
            new HorizontalOption("Death enabled", "Enables getting punished when dying", ["Off", "On"],
                setting => { _configuration.DeathPunishment.Enabled = setting == 1; },
                () => _configuration.DeathPunishment.Enabled ? 1 : 0),
            new HorizontalOption("Damage Mode", "Defines what should happen when you die",
                ["Shock", "Vibration", "Beep"],
                setting =>
                {
                    _configuration.DeathPunishment.Mode = setting switch
                    {
                        0 => PiShockConfiguration.Mode.Shock,
                        2 => PiShockConfiguration.Mode.Beep,
                        _ => PiShockConfiguration.Mode.Vibration
                    };
                },
                () => (int)_configuration.DeathPunishment.Mode),
            new CustomSlider("Duration Min",
                value => _configuration.DeathPunishment.MinDuration = Math.Round(value, 1),
                () => (float)_configuration.DeathPunishment.MinDuration,
                0, 15),
            new CustomSlider("Duration Max",
                value => _configuration.DeathPunishment.MaxDuration = Math.Round(value, 1),
                () => (float)_configuration.DeathPunishment.MaxDuration,
                0, 15),
            new CustomSlider("Intensity Min",
                value => _configuration.DeathPunishment.MinIntensity = (int)value,
                () => _configuration.DeathPunishment.MinIntensity,
                0, 100, true),
            new CustomSlider("Intensity Max",
                value => _configuration.DeathPunishment.MaxIntensity = (int)value,
                () => _configuration.DeathPunishment.MaxIntensity,
                0, 100, true),
            new TextPanel("PiShock API settings"),
            new MenuButton(
                "Show PiShock API Settings",
                "Hidden by default for privacy reasons",
                _ =>
                {
                    apiMenuVisible = !apiMenuVisible;
                    _menuRef.UpdateVisibility(apiMenuVisible,
                        ["PiShock_Username", "PiShock_APIKey", "PiShock_ConnectMode", "PiShock_ShockerCode", "PiShock_HubId", "PiShock_ShockerId"]);
                }),
            new Satchel.BetterMenus.InputField(
                "Username",
                username =>
                {
                    if (username == _configuration.PiShockSecrets.Username) return;

                    _configuration.PiShockSecrets.Username = username;
                    _configuration.PiShockSecrets.UserId = 0;
                    _handler.Restart();
                },
                () => _configuration.PiShockSecrets.Username,
                "",
                64,
                inputFieldConfig,
                "PiShock_Username"
            ) { isVisible = false },
            new Satchel.BetterMenus.InputField("API Key",
                apikey =>
                {
                    if (apikey == _configuration.PiShockSecrets.Apikey) return;

                    _configuration.PiShockSecrets.Apikey = apikey;
                    _configuration.PiShockSecrets.UserId = 0;
                    _handler.Restart();
                },
                () => _configuration.PiShockSecrets.Apikey,
                "",
                64,
                inputFieldConfig,
                "PiShock_APIKey"
            ) { isVisible = false },
            new HorizontalOption("Connection Mode", "Select how the punishment will be sent",
                ["Http", "Ws"],
                setting =>
                {
                    _configuration.PiShockSecrets.ConnectMode = setting switch
                    {
                        0 => PiShockConfiguration.ConnectMode.Http,
                        1 => PiShockConfiguration.ConnectMode.Ws,
                        _ => PiShockConfiguration.ConnectMode.Http
                    };

                    _handler.Restart();
                },
                () => (int)_configuration.PiShockSecrets.ConnectMode,
                "PiShock_ConnectMode") { isVisible = false },
            new Satchel.BetterMenus.InputField("Shocker Code (for HTTP)",
                code => _configuration.PiShockSecrets.Code = code,
                () => _configuration.PiShockSecrets.Code,
                "",
                64,
                inputFieldConfig,
                "PiShock_ShockerCode"
            ) { isVisible = false },
            new Satchel.BetterMenus.InputField("Hub Id (for WS)",
                hubId => _configuration.PiShockSecrets.HubId = Convert.ToInt32(hubId),
                () => $"{_configuration.PiShockSecrets.HubId}",
                "",
                64,
                inputFieldConfig,
                "PiShock_HubId"
            ) { isVisible = false },
            new Satchel.BetterMenus.InputField("Shocker Id (for WS)",
                shockerId => _configuration.PiShockSecrets.ShockerId = Convert.ToInt32(shockerId),
                () => $"{_configuration.PiShockSecrets.ShockerId}",
                "",
                64,
                inputFieldConfig,
                "PiShock_ShockerId"
            ) { isVisible = false }
        ]);
    }
}