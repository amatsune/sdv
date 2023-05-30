﻿namespace DaLion.Overhaul.Modules.Enchantments.Events;

#region using directives

using System.Collections.Immutable;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class EnchantmentAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <summary>Initializes a new instance of the <see cref="EnchantmentAssetsInvalidatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnchantmentAssetsInvalidatedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        Textures.Refresh(e.NamesWithoutLocale.ToImmutableHashSet());
    }
}
