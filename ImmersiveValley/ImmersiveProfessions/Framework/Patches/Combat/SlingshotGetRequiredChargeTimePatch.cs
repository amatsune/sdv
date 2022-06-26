﻿namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
    }

    #region harmony patches

    /// <summary>Patch to reduce slingshot charge time for Desperado.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer || !firer.HasProfession(Profession.Desperado)) return;
        
        __result *= 0.5f;
    }

    #endregion harmony patches
}