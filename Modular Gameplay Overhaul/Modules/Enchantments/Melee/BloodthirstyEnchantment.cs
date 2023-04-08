﻿namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Enchantments.Events;
using Microsoft.Xna.Framework;
using Shared.Extensions;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks on-hit heal for 10% of damage dealt. Excess healing is converted into a shield for up
///     to 20% of (the player's) max health, which slowly decays after not dealing or taking damage for 25s.
/// </summary>
[XmlType("Mods_DaLion_BloodthirstyEnchantment")]
public sealed class BloodthirstyEnchantment : BaseWeaponEnchantment
{
    private Random _random = new Random(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.vampiric.name");
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        var lifeSteal = Math.Max((int)(m.MaxHealth * this._random.NextFloat(0.01f, 0.05f)), 1);
        who.health = Math.Min(who.health + lifeSteal, (int)(who.maxHealth * 1.2f));
        location.debris.Add(new Debris(
            lifeSteal, 
            new Vector2(Game1.player.getStandingX(), Game1.player.getStandingY()),
            Color.Lime,
            1f,
            who));
        Game1.playSound("healSound");
        if (who.health > who.maxHealth)
        {
            EventManager.Enable<BloodthirstyUpdateTickedEvent>();
        }
    }
}