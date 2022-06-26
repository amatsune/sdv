﻿namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Objects;

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotPerformObjectDropInActionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CrabPotPerformObjectDropInActionPatch()
    {
        Target = RequireMethod<CrabPot>(nameof(CrabPot.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Patch to allow Conservationist to place bait.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CrabPotPerformObjectDropInActionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed: ... && (owner_farmer is null || !owner_farmer.professions.Contains(11)

        try
        {
            helper
                .FindProfessionCheck(Profession.Conservationist.Value)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_1)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_1)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Brtrue_S)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing Conservationist bait restriction.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}