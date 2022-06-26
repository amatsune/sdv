﻿namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;

using DaLion.Common;
using DaLion.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetProfessionTitleFromNumberPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetProfessionTitleFromNumberPatch()
    {
        Target = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getProfessionTitleFromNumber));
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession names.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
    {
        try
        {
            if (!Profession.TryFromValue(whichProfession, out var profession)) return true; // run original logic

            __result = profession.GetDisplayName(Game1.player.IsMale);
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