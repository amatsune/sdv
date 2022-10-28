﻿namespace DaLion.Redux.Core.Extensions;

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
internal static class ItemExtensions
{
    /// <summary>Determines whether the <paramref name="ammo"/> should make squishy noises upon collision.</summary>
    /// <param name="ammo">The ammo <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="ammo"/> is an egg, fruit or vegetable, otherwise <see langword="false"/>.</returns>
    internal static bool IsSquishyAmmo(this Item ammo)
    {
        return ammo.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory ||
               ammo.ParentSheetIndex == Constants.SlimeIndex;
    }
}