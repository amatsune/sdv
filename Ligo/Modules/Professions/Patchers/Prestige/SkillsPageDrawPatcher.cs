﻿namespace DaLion.Ligo.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPageDrawPatcher"/> class.</summary>
    internal SkillsPageDrawPatcher()
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Inject: x -= ModEntry.Config.Arsenal.Slingshots.PrestigeProgressionStyle == ProgressionStyle.Stars ? Textures.STARS_WIDTH_I : Textures.RIBBON_WIDTH_I;
        // After: x = ...
        try
        {
            var notRibbons = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(LocalizedContentManager).RequirePropertyGetter(nameof(LocalizedContentManager
                            .CurrentLanguageCode))))
                .AdvanceUntil(new CodeInstruction(OpCodes.Br_S))
                .GetOperand(out var resumeExecution)
                .AdvanceUntil(new CodeInstruction(OpCodes.Stloc_0))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.EnablePrestige))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.PrestigeProgressionStyle))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Beq_S, notRibbons),
                    new CodeInstruction(
                        OpCodes.Ldc_I4_S,
                        (int)((Textures.RibbonWidth + 5) * Textures.RibbonScale)),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .InsertWithLabels(
                    new[] { notRibbons },
                    new CodeInstruction(
                        OpCodes.Ldc_I4_S,
                        (int)((Textures.StarsWidth + 4) * Textures.StarsScale)),
                    new CodeInstruction(OpCodes.Sub));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adjusting localized skill page content position.\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawExtendedLevelBars(levelIndex, skillIndex, x, y, addedX, skillLevel, b)
        // Before: if (i == 9) draw level number ...
        var skillIndex = helper.Locals[4];
        var skillLevel = helper.Locals[8];
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                    new CodeInstruction(OpCodes.Bne_Un))
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_3), // load levelIndex
                    new CodeInstruction(OpCodes.Ldloc_S, skillIndex),
                    new CodeInstruction(OpCodes.Ldloc_0), // load x
                    new CodeInstruction(OpCodes.Ldloc_1), // load y
                    new CodeInstruction(OpCodes.Ldloc_2), // load addedX
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel), // load skillLevel,
                    new CodeInstruction(OpCodes.Ldarg_1), // load b
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillsPageDrawPatcher).RequireMethod(nameof(DrawExtendedLevelBars))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw skills page extended level bars.\nHelper returned {ex}");
            return null;
        }

        // From: (addedSkill ? Color.LightGreen : Color.Cornsilk)
        // To: (addedSkill ? Color.LightGreen : skillLevel == 20 ? Color.Grey : Color.SandyBrown)
        try
        {
            var isSkillLevel20 = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown))))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                    new CodeInstruction(OpCodes.Beq_S, isSkillLevel20))
                .Advance()
                .GetOperand(out var resumeExecution)
                .Advance()
                .InsertWithLabels(
                    new[] { isSkillLevel20 },
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Cornsilk))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw max skill level with different color.\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawRibbonsSubroutine(b);
        // Before: if (hoverText.Length > 0)
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).RequireField("hoverText")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(string).RequirePropertyGetter(nameof(string.Length))))
                .StripLabels(out var labels) // backup and remove branch labels
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillsPageDrawPatcher).RequireMethod(nameof(DrawRibbons))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw skills page prestige ribbons.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static void DrawExtendedLevelBars(
        int levelIndex, int skillIndex, int x, int y, int addedX, int skillLevel, SpriteBatch b)
    {
        if (!ModEntry.Config.Professions.EnablePrestige)
        {
            return;
        }

        var drawBlue = skillLevel > levelIndex + 10;
        if (!drawBlue)
        {
            return;
        }

        // this will draw only the blue bars
        if ((levelIndex + 1) % 5 != 0)
        {
            b.Draw(
                Textures.SkillBarsTx,
                new Vector2(addedX + x + (levelIndex * 36), y - 4 + (skillIndex * 56)),
                new Rectangle(0, 0, 8, 9),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                1f);
        }
    }

    internal static void DrawRibbons(SkillsPage page, SpriteBatch b)
    {
        if (!ModEntry.Config.Professions.EnablePrestige)
        {
            return;
        }

        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.ProgressionHorizontalOffset,
                page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset);
        if (ModEntry.Config.Professions.PrestigeProgressionStyle == Config.ProgressionStyle.StackedStars)
        {
            position.X -= 22;
            position.Y -= 4;
        }

        for (var i = 0; i < 5; i++)
        {
            position.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };
            var count = Game1.player.GetProfessionsForSkill(skill, true).Length;
            if (count == 0)
            {
                continue;
            }

            Rectangle srcRect;
            if (ModEntry.Config.Professions.PrestigeProgressionStyle.ToString().Contains("Ribbons"))
            {
                srcRect = new Rectangle(
                    i * Textures.RibbonWidth,
                    (count - 1) * Textures.RibbonWidth,
                    Textures.RibbonWidth,
                    Textures.RibbonWidth);
            }
            else if (ModEntry.Config.Professions.PrestigeProgressionStyle == Config.ProgressionStyle.StackedStars)
            {
                srcRect = new Rectangle(0, (count - 1) * 16, Textures.StarsWidth, 16);
            }
            else
            {
                srcRect = Rectangle.Empty;
            }

            b.Draw(
                Textures.PrestigeSheetTx,
                position,
                srcRect,
                Color.White,
                0f,
                Vector2.Zero,
                ModEntry.Config.Professions.PrestigeProgressionStyle == Config.ProgressionStyle.StackedStars
                    ? Textures.StarsScale
                    : Textures.RibbonScale,
                SpriteEffects.None,
                1f);
        }

        if (Ligo.Integrations.SpaceCoreApi is null)
        {
            return;
        }

        if (ModEntry.Config.Professions.PrestigeProgressionStyle.ToString().Contains("Ribbons"))
        {
            position.X += 2; // not sure why but custom skill ribbons render with a small offset
        }

        foreach (var skill in SCSkill.Loaded.Values)
        {
            position.Y += 56;
            var count = Game1.player.GetProfessionsForSkill(skill, true).Length;
            if (count == 0)
            {
                continue;
            }

            var srcRect = ModEntry.Config.Professions.PrestigeProgressionStyle switch
            {
                Config.ProgressionStyle.Gen3Ribbons or Config.ProgressionStyle.Gen4Ribbons => new Rectangle(
                    skill.StringId == "blueberry.LoveOfCooking.CookingSkill" ? 111 : 133,
                    (count - 1) * Textures.RibbonWidth,
                    Textures.RibbonWidth,
                    Textures.RibbonWidth),
                Config.ProgressionStyle.StackedStars =>
                    new Rectangle(0, (count - 1) * 16, Textures.StarsWidth, 16),
                _ => Rectangle.Empty,
            };

            b.Draw(
                Textures.PrestigeSheetTx,
                position,
                srcRect,
                Color.White,
                0f,
                Vector2.Zero,
                ModEntry.Config.Professions.PrestigeProgressionStyle == Config.ProgressionStyle.StackedStars
                    ? Textures.StarsScale
                    : Textures.RibbonScale,
                SpriteEffects.None,
                1f);
        }
    }

    #endregion injected subroutines
}