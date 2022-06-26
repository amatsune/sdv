﻿namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Data;
using Common.Events;
using Common.Extensions;
using Common.Extensions.Collections;

#endregion using directives

[UsedImplicitly]
internal sealed class RestoreForgottenRecipesDayStartedEvent : DayStartedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RestoreForgottenRecipesDayStartedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var forgottenRecipes = ModDataIO.ReadData(Game1.player, ModData.ForgottenRecipesDict.ToString())
            .ParseDictionary<string, int>();
        if (!forgottenRecipes.Any())
        {
            Unhook();
            return;
        }

        for (var i = forgottenRecipes.Count - 1; i >= 0; --i)
        {
            var key = forgottenRecipes.ElementAt(i).Key;
            if (Game1.player.craftingRecipes.ContainsKey(key))
            {
                Game1.player.craftingRecipes[key] += forgottenRecipes[key];
                forgottenRecipes.Remove(key);
            }
            else if (Game1.player.cookingRecipes.ContainsKey(key))
            {
                Game1.player.cookingRecipes[key] += forgottenRecipes[key];
                forgottenRecipes.Remove(key);
            }
        }

        ModDataIO.WriteData(Game1.player, ModData.ForgottenRecipesDict.ToString(), forgottenRecipes.Any()
            ? forgottenRecipes.Stringify()
            : null);
        Unhook();
    }
}