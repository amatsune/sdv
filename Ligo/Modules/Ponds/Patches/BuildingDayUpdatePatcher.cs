﻿namespace DaLion.Ligo.Modules.Ponds.Patches;

#region using directives

using DaLion.Shared.Exceptions;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class BuildingDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuildingDayUpdatePatcher"/> class.</summary>
    internal BuildingDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<Building>(nameof(Building.dayUpdate));
    }

    #region harmony patches

#if DEBUG
    /// <summary>Stub for base <see cref="FishPond.dayUpdate"/>.</summary>
    /// <remarks>Required by DayUpdate prefix.</remarks>
    [HarmonyReversePatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:Element parameters should be documented", Justification = "Reverse patch.")]
    internal static void BuildingDayUpdateReverse(object instance, int dayOfMonth)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }
#endif

    #endregion harmony patches
}