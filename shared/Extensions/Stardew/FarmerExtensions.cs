﻿namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.ModData;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Gets the tile immediately in front of the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The <see cref="Vector2"/> coordinates of the tile immediately in front of the <paramref name="farmer"/>.</returns>
    public static Vector2 GetFacingTile(this Farmer farmer)
    {
        return farmer.getTileLocation().GetNextTile(farmer.FacingDirection);
    }

    /// <summary>
    ///     Changes the <paramref name="farmer"/>'s <see cref="FacingDirection"/> in order to face the desired
    ///     <paramref name="tile"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="tile">The tile to face.</param>
    /// <returns>The new <see cref="FacingDirection"/>.</returns>
    public static FacingDirection FaceTowardsTile(this Farmer farmer, Vector2 tile)
    {
        if (!farmer.IsLocalPlayer)
        {
            ThrowHelper.ThrowInvalidOperationException("Can only do this for the local player.");
        }

        var direction = (tile - Game1.player.getTileLocation()).ToFacingDirection();
        farmer.faceDirection((int)direction);
        return direction;
    }

    /// <inheritdoc cref="ModDataIO.Read(Farmer, string, string, string)"/>
    public static string Read(this Farmer farmer, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(farmer, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(Farmer, string, T, string)"/>
    public static T Read<T>(this Farmer farmer, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(farmer, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(Farmer, string, string?)"/>
    public static void Write(this Farmer farmer, string field, string? value)
    {
        ModDataIO.Write(farmer, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(Farmer, string, string?)"/>
    public static void WriteIfNotExists(this Farmer farmer, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(farmer, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(Farmer, string, string, string)"/>
    public static void Append(this Farmer farmer, string field, string value, string separator = ",")
    {
        ModDataIO.Append(farmer, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Farmer, string, T)"/>
    public static void Increment<T>(this Farmer farmer, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(farmer, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Farmer, string, T)"/>
    public static void Increment(this Farmer farmer, string field)
    {
        ModDataIO.Increment(farmer, field, 1);
    }

    /// <summary>Counts the number of completed Monster Eradication goals.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The number of completed Monster Eradication goals.</returns>
    internal static int NumMonsterSlayerQuestsCompleted(this Farmer farmer)
    {
        var count = 0;

        if (farmer.mailReceived.Contains("Gil_Slime Charmer Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Savage Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Skeleton Mask"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Insect Head"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Vampire Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Hard Hat"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Burglar's Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Crabshell Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Arcane Hat"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Knight's Helmet"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Napalm Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Telephone"))
        {
            count++;
        }

        return count;
    }
}