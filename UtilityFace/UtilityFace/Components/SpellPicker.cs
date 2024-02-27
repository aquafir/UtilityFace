﻿using ACE.DatLoader.FileTypes;
using UtilityFace.HUDs;

namespace UtilityFace.Components;

public class SpellPicker : TexturedPicker<SpellInfo>
{
    static readonly SpellTable table = UBService.PortalDat.SpellTable;
    static readonly SpellBook spellbook = HudBase.game.Character.SpellBook;

    //Unfiltered set of choices
    static readonly SpellInfo[] choices = table.Spells.Select(x => new SpellInfo(x.Key, x.Value)).ToArray();

    SpellFilter filter = new() { Active = true, Label = "Filter Spells?" };

    public override void DrawBody()
    {
        if (filter.Check())
            this.Choices = filter.Active ? filter.GetFiltered(choices).ToArray() : choices;

        base.DrawBody();
    }

    public override void DrawItem(SpellInfo item, int index)
    {
        base.DrawItem(item, index);

        //Add in a tooltip
        if(ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            var id = item.Id;
            var spell = item.Spell;
            if (spellbook.TryGet(id, out var sd))
            {
                var skill = HudBase.game.Character.GetMagicSkill(spell.School);
                ImGui.Text($"{spell.Name} ({id})\nChance: {SkillCheck.GetMagicSkillChance(skill, (int)spell.Power):P2}%\n{spell.School}\n{spell.Power}\n{spell.Desc}");
            }

            ImGui.EndTooltip();
        }
    }

    public SpellPicker() : base(x => TextureManager.GetOrCreateTexture(x.Spell.Icon), choices) { }
}
