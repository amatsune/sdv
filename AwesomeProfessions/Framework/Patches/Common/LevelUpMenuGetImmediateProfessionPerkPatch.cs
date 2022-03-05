﻿namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

using Stardew.Common.Harmony;
using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetImmediateProfessionPerkPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to add modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (whichProfession.GetCorrespondingSkill() == SkillType.Combat)
        {
            Game1.player.maxHealth += 5;
            Game1.player.health = Game1.player.maxHealth;
        }

        if (!Enum.IsDefined(typeof(Profession), whichProfession)) return;

        var profession = (Profession) whichProfession;

        // add immediate perks
        if (profession == Profession.Aquarist)
            foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                         (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction()))
                pond.UpdateMaximumOccupancy();

        // subscribe events
        EventManager.EnableAllForProfession(profession);
        if (profession == Profession.Conservationist && !Context.IsMainPlayer) // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage("Conservationism", "RequestEvent",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});

        if (whichProfession is < 26 or >= 30 || ModEntry.PlayerState.Value.RegisteredUltimate is not null) return;
        
        // register Ultimate
        var newIndex = (UltimateIndex) whichProfession;
        ModEntry.PlayerState.Value.RegisteredUltimate =
#pragma warning disable CS8509
            ModEntry.PlayerState.Value.RegisteredUltimate = newIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.Brute => new Frenzy(),
                UltimateIndex.Poacher => new Ambush(),
                UltimateIndex.Piper => new Pandemonia(),
                UltimateIndex.Desperado => new DeathBlossom()
            };
        Game1.player.WriteData(DataField.UltimateIndex, newIndex.ToString());
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: case <defender_id>:
        /// To: case <brute_id>:

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
                )
                .SetOperand((int) Profession.Brute);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}