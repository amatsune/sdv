﻿namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using Extensions;
using GameLoop;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BruteWarpedEvent : WarpedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal BruteWarpedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            Manager.Enable<BruteUpdateTickedEvent>();
        }
        else
        {
            ModEntry.State.BruteRageCounter = 0;
            Manager.Enable<BruteUpdateTickedEvent>();
        }
    }
}