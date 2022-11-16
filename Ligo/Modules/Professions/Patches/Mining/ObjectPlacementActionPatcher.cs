﻿namespace DaLion.Ligo.Modules.Professions.Patches.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewModdingAPI.Utilities;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlacementActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPlacementActionPatcher"/> class.</summary>
    internal ObjectPlacementActionPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.placementAction));
    }

    #region harmony patches

    /// <summary>Patch to prevent quantum bombs when detonating manually.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectPlacementActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (who is not null && who.professions.Contains(<demolitionist_id>) && ModEntry.Config.Arsenal.Slingshots.ModKey.IsDown()) skipIntensity ...
        // After: new TemporaryAnimatedSprite( ... )
        var i = 0;
        repeat:
        try
        {
            var skipIntensity = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    new CodeInstruction(
                        OpCodes.Stfld,
                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite.shakeIntensity))))
                .AddLabels(resumeExecution)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4))
                .InsertProfessionCheck(Profession.Demolitionist.Value, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.ModKey))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(KeybindList).RequireMethod(nameof(KeybindList.IsDown))),
                    new CodeInstruction(OpCodes.Brtrue_S, skipIntensity))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(
                        OpCodes.Stfld,
                        typeof(TemporaryAnimatedSprite).RequireField(nameof(TemporaryAnimatedSprite
                            .extraInfoForEndBehavior))))
                .AddLabels(skipIntensity);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting intensity skip for manually-detonated bombs.\nHelper returned {ex}");
            return null;
        }

        // repeat injection three times
        if (++i < 3)
        {
            goto repeat;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}