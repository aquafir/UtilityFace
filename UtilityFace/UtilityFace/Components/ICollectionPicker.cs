namespace UtilityFace.Components;

public abstract class ICollectionPicker<T> : IPicker<T> //where T
{
    //Debated about where to put this.  Some choices till be different, like string[] for Combos and mapped to the generic type
    /// <summary>
    /// Items being displayed and selected from
    /// </summary>
    //public IEnumerable<T> Choices;
    public T[] Choices;

    //How to handle layout?
    //Vector2 Size / float Width / etc.

    public override void DrawBody()
    {
        //Index used for line breaks?
        int index = 0;

        foreach (var choice in Choices)
            DrawItem(choice, index++);
    }

    public abstract void DrawItem(T item, int index);
}

//public override void DrawBody()
//{
//    var width = ImGui.GetWindowWidth() - IconSize.X / 2;
//    var margin = ImGui.GetStyle().FramePadding.X;
//    var colWidth = 1 + IconSize.X + margin * 2;
//    int cols = (int)(width / colWidth);

//    int index = 0;


//    uint start = (uint)(page * perPage);

//    ImGui.Text($"Page {page}/{Pages}, start at index {start}. {cols} x {colWidth} cols");

//    if (Spells.Length == 0)
//        return;

//    for (uint i = start; i < start + perPage; i++)
//    {
//        if (index++ % cols != 0)
//            ImGui.SameLine();

//        //if (!table.Spells.TryGetValue(i, out var spell))
//        //{
//        //    ImGui.TextureButton($"Spell{i}", TextureManager.GetOrCreateTexture(Enums.Texture.Vitae), IconSize);
//        //    continue;
//        //}
//        var spell = Spells[i].Value;
//        var id = Spells[i].Key;

//        var tex = TextureManager.GetOrCreateTexture(spell.Icon);
//        if (ImGui.TextureButton($"Spell{i}", tex, IconSize))
//            Log.Chat($"Clicked {spell.Name}");

//        if (ImGui.IsItemHovered())
//        {
//            ImGui.BeginTooltip();
//            if (spellbook.TryGet(id, out var sd))
//            {
//                var skill = HudBase.game.Character.GetMagicSkill(spell.School);
//                ImGui.Text($"{spell.Name} ({id})\nChance: {SkillCheck.GetMagicSkillChance(skill, (int)spell.Power):P2}%\n{spell.School}\n{spell.Power}\n{spell.Desc}");
//            }

//            ImGui.EndTooltip();
//        }
//    }
//}

////Calculate the size
////var width = ImGui.GetWindowWidth() - IconSize.X / 2;
////var margin = ImGui.GetStyle().FramePadding.X;
////var colWidth = 1 + IconSize.X + margin * 2;
////int cols = (int)(width / colWidth);

