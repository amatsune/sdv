﻿namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

using Common.Events;

#endregion using directives

internal sealed class UltimateEmptiedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateEmptiedEventArgs> _OnEmptiedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateEmptiedEvent(Action<object?, IUltimateEmptiedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnEmptiedImpl = callback;
    }

    /// <summary>Raised when the local player's ultimate charge value returns to zero.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEmptied(object? sender, IUltimateEmptiedEventArgs e)
    {
        if (Hooked.Value) _OnEmptiedImpl(sender, e);
    }
}