﻿namespace DaLion.Stardew.Tools.Commands;

#region using directives

using Common;
using Common.Commands;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class AddEnchantmentCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddEnchantmentCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "add_enchants", "add", "enchant" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified enchantment to the selected tool." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            Log.W("You must select a weapon first.");
            return;
        }

        while (args.Length > 0)
        {
            BaseEnchantment? enchantment = args[0].ToLower() switch
            {
                "auto-hook" or "autohook" => new AutoHookEnchantment(),
                "archaeologist" => new ArchaeologistEnchantment(),
                "bottomless" => new BottomlessEnchantment(),
                "efficient" => new EfficientToolEnchantment(),
                "generous" => new GenerousEnchantment(),
                "master" => new MasterEnchantment(),
                "powerful" => new PowerfulEnchantment(),
                "preserving" => new PreservingEnchantment(),
                "reaching" => new ReachingToolEnchantment(),
                "shaving" => new ShavingEnchantment(),
                "swift" => new SwiftToolEnchantment(),
                _ => null
            };

            if (enchantment is null)
            {
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                return;
            }

            if (!enchantment.CanApplyTo(weapon))
            {
                Log.W($"Cannot apply {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");
                return;
            }

            weapon.enchantments.Add(enchantment);
            Log.I($"Applied {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");

            args = args.Skip(1).ToArray();
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Triggers.First()} <enchantment>";
        result += "\n\nParameters:";
        result += "\n\t- <enchantment>: a tool enchantment";
        result += "\n\nExample:";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.First()} powerful";
        return result;
    }
}