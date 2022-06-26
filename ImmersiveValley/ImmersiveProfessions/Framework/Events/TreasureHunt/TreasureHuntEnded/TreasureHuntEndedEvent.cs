﻿namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;

using Common.Events;

#endregion using directives

internal sealed class TreasureHuntEndedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntEndedEventArgs> _OnEndedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal TreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnEndedImpl = callback;
    }

    /// <summary>Raised when a Treasure Hunt ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEnded(object? sender, ITreasureHuntEndedEventArgs e)
    {
        if (Hooked.Value) _OnEndedImpl(sender, e);
    }
}