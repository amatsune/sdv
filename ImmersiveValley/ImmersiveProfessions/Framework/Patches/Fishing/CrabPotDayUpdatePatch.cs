﻿namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

using DaLion.Common;
using DaLion.Common.Data;
using DaLion.Common.Extensions;
using DaLion.Common.Harmony;
using Extensions;

using SUtility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotDayUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CrabPotDayUpdatePatch()
    {
        Target = RequireMethod<CrabPot>(nameof(CrabPot.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
    [HarmonyPrefix]
    private static bool CrabPotDayUpdatePrefix(CrabPot __instance, GameLocation location)
    {
        try
        {
            var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
            var isConservationist = owner.HasProfession(Profession.Conservationist);
            if (__instance.bait.Value is null && !isConservationist || __instance.heldObject.Value is not null)
                return false; // don't run original logic

            var r = new Random(Guid.NewGuid().GetHashCode());
            var fishData =
                Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"));
            var isLuremaster = owner.HasProfession(Profession.Luremaster);
            var whichFish = -1;
            if (__instance.bait.Value is not null)
            {
                if (isLuremaster)
                {
                    if (__instance.HasMagnet())
                    {
                        whichFish = __instance.ChoosePirateTreasure(owner, r);
                    }
                    else if (Game1.random.NextDouble() < 0.25)
                    {
                        whichFish = __instance.ChooseFish(fishData, location, r);
                        if (whichFish < 0) whichFish = __instance.ChooseTrapFish(fishData, location, r, true);
                    }
                    else
                    {
                        whichFish = __instance.ChooseTrapFish(fishData, location, r, true);
                    }
                }
                else
                {
                    whichFish = __instance.ChooseTrapFish(fishData, location, r, false);
                }
            }

            var fishQuality = 0;
            var fishQuantity = 1;
            if (whichFish < 0)
            {
                if (__instance.bait.Value is not null || isConservationist)
                {
                    whichFish = __instance.GetTrash(location, r);
                    if (isConservationist && whichFish.IsTrash())
                    {
                        ModDataIO.IncrementData<uint>(owner,
                            ModData.ConservationistTrashCollectedThisSeason.ToString());
                        if (owner.HasProfession(Profession.Conservationist, true) && ModDataIO.ReadDataAs<uint>(owner,
                                ModData.ConservationistTrashCollectedThisSeason.ToString()) %
                            ModEntry.Config.TrashNeededPerFriendshipPoint == 0)
                            SUtility.improveFriendshipWithEveryoneInRegion(owner, 1, 2);
                    }
                }
                else
                {
                    return false; // don't run original logic
                }
            }
            else if (!whichFish.IsIn(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534)) // not ring or weapon
            {
                fishQuality = __instance.GetTrapQuality(whichFish, owner, r, isLuremaster);
                fishQuantity = __instance.GetTrapQuantity(whichFish, owner, r);
            }

            __instance.heldObject.Value = new(whichFish, fishQuantity, quality: fishQuality);
            __instance.tileIndexToShow = 714;
            __instance.readyForHarvest.Value = true;

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}