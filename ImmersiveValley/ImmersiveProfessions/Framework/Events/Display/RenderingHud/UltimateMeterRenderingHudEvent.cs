﻿namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateMeterRenderingHudEvent : RenderingHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateMeterRenderingHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Unhook();
            return;
        }

        if (!Game1.eventUp) ModEntry.PlayerState.RegisteredUltimate.Hud.Draw(e.SpriteBatch);
    }
}