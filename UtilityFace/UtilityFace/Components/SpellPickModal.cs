using ACE.DatLoader.Entity;
using UtilityFace.HUDs;

namespace UtilityFace.Components;
public class SpellPickModal() : IModal()
{
    static readonly SpellBook spellbook = HudBase.game.Character.SpellBook;

    int page = 0;
    int perPage = 20;
    int Pages => Spells.Length > 0 ? (Spells.Length / perPage) + 1 : 0;

    SpellFilter filter = new() {  Active = true, Label = "Filter?" };
    SpellInfo[] Spells = { };

    public Vector2 IconSize = new(24);


    public override void Init()
    {
        MinSize = new(300);
        Spells = filter.GetFilteredSpells();
    }

    public override void DrawBody()
    {
        var width = ImGui.GetWindowWidth() - IconSize.X / 2;
        var margin = ImGui.GetStyle().FramePadding.X;
        var colWidth = 1 + IconSize.X + margin * 2;
        int cols = (int)(width / colWidth);

        int index = 0;
        if (filter.Check())
        {
            page = 0;
            Spells = filter.GetFilteredSpells();
        }

        ImGui.SliderInt("Page", ref page, 0, Pages);

        uint start = (uint)(page * perPage);

        ImGui.Text($"Page {page}/{Pages}, start at index {start}. {cols} x {colWidth} cols");

        if (Spells.Length == 0)
            return;

        for (uint i = start; i < start + perPage; i++)
        {
            if (index++ % cols != 0)
                ImGui.SameLine();

            //if (!table.Spells.TryGetValue(i, out var spell))
            //{
            //    ImGui.TextureButton($"Spell{i}", TextureManager.GetOrCreateTexture(Enums.Texture.Vitae), IconSize);
            //    continue;
            //}
            var spell = Spells[i].Spell;
            var id = Spells[i].Id;

            var tex = TextureManager.GetOrCreateTexture(spell.Icon);
            if (ImGui.TextureButton($"Spell{i}", tex, IconSize))
                Log.Chat($"Clicked {spell.Name}");

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                if (spellbook.TryGet(id, out var sd))
                {
                    var skill = HudBase.game.Character.GetMagicSkill(spell.School);
                    ImGui.Text($"{spell.Name} ({id})\nChance: {SkillCheck.GetMagicSkillChance(skill, (int)spell.Power):P2}%\n{spell.School}\n{spell.Power}\n{spell.Desc}");
                }

                ImGui.EndTooltip();
            }
        }

        //if (ImGui.Button("Close"))
        //    _open = false;
    }
}
