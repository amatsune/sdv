﻿namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to make Poacher invulnerable in Ambuscade + remove vanilla defense cap + make Brute unkillable in Frenzy
    ///     + increment Brute rage counter and ultimate meter.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: else if (this.IsLocalPlayer && this.get_Ultimate() is Ambush {IsActive: true}) monsterDamageCapable = false;
        try
        {
            var alreadyUndamageableOrNotAmbuscade = generator.DefineLabel();
            var ambush = generator.DeclareLocal(typeof(Ambush));
            helper
                .FindFirst(new CodeInstruction(OpCodes.Stloc_0))
                .Advance()
                .AddLabels(alreadyUndamageableOrNotAmbuscade)
                .InsertInstructions(
                    // check if monsterDamageCapable is already false
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check if this is the local player
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check for ambush
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Ambush)),
                    new CodeInstruction(OpCodes.Stloc_S, ambush),
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // check if it's active
                    new CodeInstruction(OpCodes.Ldloc_S, ambush),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Ultimate).RequirePropertyGetter(nameof(Ultimate.IsActive))),
                    new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
                    // set monsterDamageCapable = false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_0));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Poacher Ambush untargetability.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (this.IsLocalPlayer && this.get_Ultimate() is Frenzy {IsActive: true}) health = 1;
        // After: if (health <= 0)
        // Before: GetEffectsOfRingMultiplier(863)
        var frenzy = generator.DeclareLocal(typeof(Frenzy));
        try
        {
            var isNotUndyingButMayHaveDailyRevive = generator.DefineLabel();
            helper
                .FindNext(
                    // find index of health <= 0 (start of revive ring effect)
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(Farmer).RequireField(nameof(Farmer.health))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bgt))
                .AdvanceUntil(new CodeInstruction(OpCodes.Bgt))
                .GetOperand(out var resumeExecution) // copy branch label to resume normal execution
                .Advance()
                .AddLabels(isNotUndyingButMayHaveDailyRevive)
                .InsertInstructions(
                    // check if this is the local player
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // check for frenzy
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Frenzy)),
                    new CodeInstruction(OpCodes.Stloc_S, frenzy),
                    new CodeInstruction(OpCodes.Ldloc, frenzy),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // check if it's active
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.IsActive))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
                    // set health back to 1
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(
                        OpCodes.Stfld,
                        typeof(Farmer).RequireField(nameof(Farmer.health))),
                    // resume execution (skip revive ring effect)
                    new CodeInstruction(OpCodes.Br, resumeExecution));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Brute Frenzy immortality.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (this.IsLocalPlayer && this.professions.Contains(<brute_id>) && damager is not null)
        //              var frenzy = this.Get_Ultimate() as Frenzy;
        //              this.Increment_BruteRageCounter(frenzy?.IsActive ? 2 : 1));
        //              if (!frenzy.IsActive) frenzy.ChargeValue += damage / 4.0;
        // At: end of method (before return)
        var isActive = generator.DeclareLocal(typeof(bool));
        try
        {
            var resumeExecution = generator.DefineLabel();
            var doesNotHaveFrenzyOrIsNotActive = generator.DefineLabel();
            var increment = generator.DefineLabel();
            helper
                .FindLast(new CodeInstruction(OpCodes.Ret)) // find index of final return
                .AddLabels(resumeExecution) // branch here to skip increments
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldarg_0))
                .InsertProfessionCheck(Profession.Brute.Value, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check if damager null
                    new CodeInstruction(OpCodes.Ldarg_3), // arg 3 = Monster damager
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check for frenzy
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                    new CodeInstruction(OpCodes.Isinst, typeof(Frenzy)),
                    new CodeInstruction(OpCodes.Stloc_S, frenzy),
                    // increment rage counter
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Brfalse_S, doesNotHaveFrenzyOrIsNotActive),
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.IsActive))),
                    new CodeInstruction(OpCodes.Stloc_S, isActive),
                    new CodeInstruction(OpCodes.Ldloc_S, isActive),
                    new CodeInstruction(OpCodes.Brfalse_S, doesNotHaveFrenzyOrIsNotActive),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, increment))
                .InsertWithLabels(
                    new[] { doesNotHaveFrenzyOrIsNotActive },
                    new CodeInstruction(OpCodes.Ldc_I4_1))
                .InsertWithLabels(
                    new[] { increment },
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Farmer_BruteCounters).RequireMethod(nameof(Farmer_BruteCounters.Increment_BruteRageCounter))),
                    // check frenzy once again
                    new CodeInstruction(OpCodes.Ldloc_S, isActive),
                    new CodeInstruction(OpCodes.Brtrue_S, resumeExecution),
                    // increment ultimate meter
                    new CodeInstruction(OpCodes.Ldloc_S, frenzy),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(IUltimate).RequirePropertyGetter(nameof(IUltimate.ChargeValue))),
                    new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldc_R8, 4d),
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(IUltimate).RequirePropertySetter(nameof(IUltimate.ChargeValue))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while incrementing Brute rage counter and ultimate meter.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}