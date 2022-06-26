﻿namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ScavengerHuntDayStartedEvent : DayStartedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ScavengerHuntDayStartedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        if (ModEntry.PlayerState.ScavengerHunt is not null)
            ModEntry.PlayerState.ScavengerHunt.ResetChanceAccumulator();
    }
}