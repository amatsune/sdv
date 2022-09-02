﻿namespace DaLion.Stardew.Tweex.Extensions;

#region using directives

using Common.Extensions.Stardew;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Whether a given object is a bee house.</summary>
    public static bool IsBeeHouse(this SObject @object) =>
        @object.bigCraftable.Value && @object.ParentSheetIndex == 10;

    /// <summary>Whether a given object is a mushroom box.</summary>
    public static bool IsMushroomBox(this SObject @object) =>
        @object.bigCraftable.Value && @object.ParentSheetIndex == 128;

    /// <summary>Whether a given object is a gem or mineral.</summary>
    public static bool IsPreciousRock(this SObject @object) =>
        @object.Category is SObject.GemCategory or SObject.mineralsCategory;

    /// <summary>Get an object quality value based on this object's age.</summary>
    public static int GetQualityFromAge(this SObject @object)
    {
        var skillFactor = 1f + Game1.player.FarmingLevel * 0.1f;
        var age = (int)(@object.Read<int>("Age") * skillFactor * ModEntry.Config.AgeImproveQualityFactor);

        if (ModEntry.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality
            };
        }

        return Game1.random.Next(age) switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality
        };
    }

    /// <summary>Whether a given object is an artisan good.</summary>
    public static bool IsArtisanGood(this SObject @object) =>
        @object.Category is SObject.artisanGoodsCategory or SObject.syrupCategory || @object.ParentSheetIndex == 395; // exception for coffee
}