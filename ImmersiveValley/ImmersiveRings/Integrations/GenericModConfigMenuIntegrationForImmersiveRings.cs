namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;
using System;
using System.Linq;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Rings.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveRings
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveRings(IModRegistry modRegistry, IManifest manifest,
        Func<ModConfig> getConfig, Action reset, Action saveAndApply)
    {
        _configMenu = new(modRegistry, manifest, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!_configMenu.IsLoaded)
            return;

        // register
        _configMenu
            .Register()
            .AddCheckbox(
                () => "Rebalanced Rings",
                () => "Improves certain underwhelming rings.",
                config => config.RebalancedRings,
                (config, value) =>
                {
                    config.RebalancedRings = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/ObjectInformation");
                }
            )
            .AddDropdown(
                () => "Topaz Perk",
                () => "The stat improved by the Topaz Ring.\nYOU MUST UNEQUIP ALL EXISTING TOPAZ RINGS OR TOPAZ-FORGED IRIDIUM BANDS BEFORE CHANGING THIS SETTING.",
                config => config.TopazPerk.ToString(),
                (config, value) =>
                {
                    config.TopazPerk = Enum.Parse<ModConfig.Perk>(value);
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/ObjectInformation");
                },
                Enum.GetValues<ModConfig.Perk>().Select(p => p.ToString()).ToArray(),
                null
            )
            .AddCheckbox(
                () => "Craftable Gemstone Rings",
                () => "Adds new combat recipes for crafting gemstone rings.",
                config => config.CraftableGemRings,
                (config, value) =>
                {
                    config.CraftableGemRings = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/CraftingRecipes");
                    ModEntry.ModHelper.GameContent.InvalidateCache("Maps/springobjects");
                })
            .AddCheckbox(
                () => "Craftable Glow and Magnet Rings",
                () => "Adds new mining recipes for crafting glow and magnet rings.",
                config => config.CraftableGlowAndMagnetRings,
                (config, value) =>
                {
                    config.CraftableGlowAndMagnetRings = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/CraftingRecipes");
                })
            .AddCheckbox(
                () => "Immersive Glowstone Recipe",
                () => "Replaces the glowstone ring recipe with one that makes sense.",
                config => config.ImmersiveGlowstoneRecipe,
                (config, value) =>
                {
                    config.ImmersiveGlowstoneRecipe = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/CraftingRecipes");
                })
            .AddCheckbox(
                () => "The One Iridium Band",
                () => "Replaces the iridium band recipe and effect. Adds new forge mechanics.",
                config => config.TheOneIridiumBand,
                (config, value) =>
                {
                    config.TheOneIridiumBand = value;
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/CraftingRecipes");
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/ObjectInformation");
                    ModEntry.ModHelper.GameContent.InvalidateCache("Maps/springobjects");
                });
    }
}