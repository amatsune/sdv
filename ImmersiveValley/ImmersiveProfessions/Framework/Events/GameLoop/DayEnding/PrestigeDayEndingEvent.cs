﻿namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Events;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeDayEndingEvent : DayEndingEvent
{
    private static Queue<ISkill> _ToReset => ModEntry.PlayerState.SkillsToReset;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PrestigeDayEndingEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        while (_ToReset.Any())
        {
            var toReset = _ToReset.Dequeue();
            switch (toReset)
            {
                case Skill skill:
                    Game1.player.ResetSkill(skill);
                    break;
                case CustomSkill customSkill:
                    Game1.player.ResetCustomSkill(customSkill);
                    break;
            }
        }

        Unhook();
    }
}