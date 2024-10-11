using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using System.Text.Json.Serialization;
using StoreApi;
using CounterStrikeSharp.API;

namespace KillsRewards;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = "{silver}- {red}[ Store ]{silver} -";

    [JsonPropertyName("GiveCreditsBasedOnDistance")]
    public bool GiveCreditsBasedOnDistance { get; set; } = true;

    [JsonPropertyName("GiveCredits")]
    public bool GiveCredits { get; set; } = true;

    [JsonPropertyName("MidAirCredits")]
    public int MidAirCredits { get; set; } = 10;

    [JsonPropertyName("MidAirNoScopeCredits")]
    public int MidAirNoScopeCredits { get; set; } = 20;

    [JsonPropertyName("NoScopeCredits")]
    public int NoScopeCredits { get; set; } = 10;

    [JsonPropertyName("HeadshotCredits")]
    public int HeadshotCredits { get; set; } = 20;

    [JsonPropertyName("NoScopeHeadshotCredits")]
    public int NoScopeHeadshotCredits { get; set; } = 30;

    [JsonPropertyName("ThruSmokeCredits")]
    public int ThruSmokeCredits { get; set; } = 50;

    [JsonPropertyName("ThruSmokeNoScopeCredits")]
    public int ThruSmokeNoScopeCredits { get; set; } = 60;

    [JsonPropertyName("ThruSmokeHeadshotCredits")]
    public int ThruSmokeHeadshotCredits { get; set; } = 70;

    [JsonPropertyName("ThruSmokeNoScopeHeadshotCredits")]
    public int ThruSmokeNoScopeHeadshotCredits { get; set; } = 80;

    [JsonPropertyName("FlashBanggedCredits")]
    public int FlashBanggedCredits { get; set; } = 40;

    [JsonPropertyName("FlashBangThruSmokeCredits")]
    public int FlashBangThruSmokeCredits { get; set; } = 90;

    [JsonPropertyName("FlashBangHeadshotCredits")]
    public int FlashBangHeadshotCredits { get; set; } = 100;

    [JsonPropertyName("FlashBangNoScopeCredits")]
    public int FlashBangNoScopeCredits { get; set; } = 110;

    [JsonPropertyName("FlashBangNoScopeHeadshotCredits")]
    public int FlashBangNoScopeHeadshotCredits { get; set; } = 120;
    public class Settings
    {
        [JsonPropertyName("EnableMidAir")]
        public bool EnableMidAir { get; set; } = true;

        [JsonPropertyName("EnableMidAirNoScope")]
        public bool EnableMidAirNoScope { get; set; } = true;

        [JsonPropertyName("EnableNoScope")]
        public bool EnableNoScope { get; set; } = true;

        [JsonPropertyName("EnableHeadshot")]
        public bool EnableHeadshot { get; set; } = true;

        [JsonPropertyName("EnableNoScopeHeadshot")]
        public bool EnableNoScopeHeadshot { get; set; } = true;

        [JsonPropertyName("EnableThruSmoke")]
        public bool EnableThruSmoke { get; set; } = true;

        [JsonPropertyName("EnableThruSmokeNoScope")]
        public bool EnableThruSmokeNoScope { get; set; } = true;

        [JsonPropertyName("EnableThruSmokeHeadshot")]
        public bool EnableThruSmokeHeadshot { get; set; } = true;

        [JsonPropertyName("EnableThruSmokeNoScopeHeadshot")]
        public bool EnableThruSmokeNoScopeHeadshot { get; set; } = true;

        [JsonPropertyName("EnableFlashBangged")]
        public bool EnableFlashBangged { get; set; } = true;

        [JsonPropertyName("EnableFlashBangThruSmoke")]
        public bool EnableFlashBangThruSmoke { get; set; } = true;

        [JsonPropertyName("EnableFlashBangHeadshot")]
        public bool EnableFlashBangHeadshot { get; set; } = true;

        [JsonPropertyName("EnableFlashBangNoScope")]
        public bool EnableFlashBangNoScope { get; set; } = true;

        [JsonPropertyName("EnableFlashBangNoScopeHeadshot")]
        public bool EnableFlashBangNoScopeHeadshot { get; set; } = true;
    }

    [JsonPropertyName("Settings")]
    public Settings ConfigSettings { get; set; } = new Settings();
}


public class KillsRewards : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleAuthor => "T3Marius";
    public override string ModuleName => "[Store Module] Kills Rewards";
    public override string ModuleVersion => "1.0";
    public PluginConfig Config { get; set; } = new PluginConfig();

    private IStoreApi? StoreApi;

    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
        config.Prefix = StringExtensions.ReplaceColorTags(config.Prefix);
    }
    public override void Load(bool hotReload)
    {
        StoreApi = IStoreApi.Capability.Get();
        if (StoreApi == null)
        {
            throw new Exception("Store api not found!");
        }
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var Attacker = @event.Attacker;
        var Victim = @event.Userid;
        int Distance = (int)Math.Round(@event.Distance);
        int creditsToGive = Distance;

        if (Attacker == null || Victim == null)
            return HookResult.Continue;

        if (@event.Attackerinair && Config.ConfigSettings.EnableMidAir)
        {
            if (@event.Noscope && Config.ConfigSettings.EnableMidAirNoScope)
            {
                if (Config.GiveCredits)
                {
                    if (Config.GiveCreditsBasedOnDistance)
                    {
                        StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 4);
                        Server.PrintToChatAll(Config.Prefix + Localizer["MidAirNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive]);
                    }
                    else
                    {
                        StoreApi?.GivePlayerCredits(Attacker, Config.MidAirNoScopeCredits);
                        Server.PrintToChatAll(Config.Prefix + Localizer["MidAirNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, Config.MidAirNoScopeCredits]);
                    }
                }
                else
                {
                    Server.PrintToChatAll(Config.Prefix + Localizer["MidAirNoScopeNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
                }
            }
            else
            {
                if (Config.GiveCredits)
                {
                    if (Config.GiveCreditsBasedOnDistance)
                    {
                        StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 2);
                        Server.PrintToChatAll(Config.Prefix + Localizer["MidAir", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive]);
                    }
                    else
                    {
                        StoreApi?.GivePlayerCredits(Attacker, Config.MidAirCredits);
                        Server.PrintToChatAll(Config.Prefix + Localizer["MidAir", Attacker.PlayerName, Victim.PlayerName, Distance, Config.MidAirCredits]);
                    }
                }
                else
                {
                    Server.PrintToChatAll(Config.Prefix + Localizer["MidAirNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
                }
            }
        }

        if (@event.Thrusmoke && Config.ConfigSettings.EnableThruSmoke)
        {
            if (@event.Noscope && Config.ConfigSettings.EnableThruSmokeNoScope && @event.Headshot && Config.ConfigSettings.EnableThruSmokeNoScopeHeadshot)
            {
                if (Config.GiveCredits)
                {
                    if (Config.GiveCreditsBasedOnDistance)
                    {
                        StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 5);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeNoScopeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                    }
                    else
                    {
                        StoreApi?.GivePlayerCredits(Attacker, Config.ThruSmokeNoScopeHeadshotCredits);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeNoScopeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, Config.ThruSmokeNoScopeHeadshotCredits]);
                    }
                }
                else
                {
                    Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeNoScopeHeadshotNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
                }
                return HookResult.Continue;
            }

            if (@event.Headshot && Config.ConfigSettings.EnableThruSmokeHeadshot)
            {
                if (Config.GiveCredits)
                {
                    if (Config.GiveCreditsBasedOnDistance)
                    {
                        StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 4);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                    }
                    else
                    {
                        StoreApi?.GivePlayerCredits(Attacker, Config.ThruSmokeHeadshotCredits);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, Config.ThruSmokeHeadshotCredits]);
                    }
                }
                else
                {
                    Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeHeadshotNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
                }
                return HookResult.Continue;
            }

            if (@event.Noscope && Config.ConfigSettings.EnableThruSmokeNoScope)
            {
                if (Config.GiveCredits)
                {
                    if (Config.GiveCreditsBasedOnDistance)
                    {
                        StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 3);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                    }
                    else
                    {
                        StoreApi?.GivePlayerCredits(Attacker, Config.ThruSmokeNoScopeCredits);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, Config.ThruSmokeNoScopeCredits]);
                    }
                }
                else
                {
                    Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeNoScopeNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
                }
                return HookResult.Continue;
            }

            if (Config.ConfigSettings.EnableThruSmoke)
            {
                if (Config.GiveCredits)
                {
                    if (Config.GiveCreditsBasedOnDistance)
                    {
                        StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 2);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmoke", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                    }
                    else
                    {
                        StoreApi?.GivePlayerCredits(Attacker, Config.ThruSmokeCredits);
                        Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmoke", Attacker.PlayerName, Victim.PlayerName, Distance, Config.ThruSmokeCredits]);
                    }
                }
                else
                {
                    Server.PrintToChatAll(Config.Prefix + Localizer["ThruSmokeNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
                }
                return HookResult.Continue;
            }
        }
        if (@event.Headshot && @event.Noscope && Config.ConfigSettings.EnableNoScopeHeadshot)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 2);
                    Server.PrintToChatAll(Config.Prefix + Localizer["HeadshotNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.NoScopeHeadshotCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["HeadshotNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, Config.NoScopeHeadshotCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["HeadshotNoScopeNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }

        if (@event.Headshot && Config.ConfigSettings.EnableHeadshot)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive);
                    Server.PrintToChatAll(Config.Prefix + Localizer["Headshot", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.HeadshotCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["Headshot", Attacker.PlayerName, Victim.PlayerName, Distance, Config.HeadshotCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["HeadshotNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }

        if (@event.Noscope && Config.ConfigSettings.EnableNoScope)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive);
                    Server.PrintToChatAll(Config.Prefix + Localizer["NoScope", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.NoScopeCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["NoScope", Attacker.PlayerName, Victim.PlayerName, Distance, Config.NoScopeCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["NoScopeNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }

        if (@event.Attackerblind && @event.Thrusmoke && @event.Noscope && @event.Headshot && Config.ConfigSettings.EnableFlashBangNoScopeHeadshot)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 6);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmokeNoScopeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.FlashBangNoScopeHeadshotCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmokeNoScopeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, Config.FlashBangNoScopeHeadshotCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmokeNoScopeHeadshotNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }

        if (@event.Attackerblind && @event.Thrusmoke && @event.Headshot && Config.ConfigSettings.EnableFlashBangHeadshot)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 5);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmokeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.FlashBangHeadshotCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmokeHeadshot", Attacker.PlayerName, Victim.PlayerName, Distance, Config.FlashBangHeadshotCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmokeHeadshotNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }

        if (@event.Attackerblind && @event.Noscope && Config.ConfigSettings.EnableFlashBangNoScope)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 4);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.FlashBangNoScopeCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangNoScope", Attacker.PlayerName, Victim.PlayerName, Distance, Config.FlashBangNoScopeCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangNoScopeNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }

        if (@event.Attackerblind && @event.Thrusmoke && Config.ConfigSettings.EnableFlashBangThruSmoke)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 3);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmoke", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive * 2]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.FlashBangThruSmokeCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmoke", Attacker.PlayerName, Victim.PlayerName, Distance, Config.FlashBangThruSmokeCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangThruSmokeNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }

        if (@event.Attackerblind && Config.ConfigSettings.EnableFlashBangged)
        {
            if (Config.GiveCredits)
            {
                if (Config.GiveCreditsBasedOnDistance)
                {
                    StoreApi?.GivePlayerCredits(Attacker, creditsToGive * 2);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBang", Attacker.PlayerName, Victim.PlayerName, Distance, creditsToGive]);
                }
                else
                {
                    StoreApi?.GivePlayerCredits(Attacker, Config.FlashBanggedCredits);
                    Server.PrintToChatAll(Config.Prefix + Localizer["FlashBang", Attacker.PlayerName, Victim.PlayerName, Distance, Config.FlashBanggedCredits]);
                }
            }
            else
            {
                Server.PrintToChatAll(Config.Prefix + Localizer["FlashBangNoCredits", Attacker.PlayerName, Victim.PlayerName, Distance]);
            }
            return HookResult.Continue;
        }
        return HookResult.Continue;
    }
}