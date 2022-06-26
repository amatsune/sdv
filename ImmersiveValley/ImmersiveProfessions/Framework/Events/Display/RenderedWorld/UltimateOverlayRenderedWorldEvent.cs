﻿namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateOverlayRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateOverlayRenderedWorldEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Unhook();
            return;
        }

        ModEntry.PlayerState.RegisteredUltimate.Overlay.Draw(e.SpriteBatch);
    }
}