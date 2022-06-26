﻿namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

using Common.Events;

#endregion using directives

internal sealed class UltimateChargeIncreasedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateChargeIncreasedEventArgs> _OnChargeIncreasedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateChargeIncreasedEvent(Action<object?, IUltimateChargeIncreasedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnChargeIncreasedImpl = callback;
    }

    /// <summary>Raised when a player's combat Ultimate gains any charge.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeIncreased(object? sender, IUltimateChargeIncreasedEventArgs e)
    {
        if (Hooked.Value) _OnChargeIncreasedImpl(sender, e);
    }
}