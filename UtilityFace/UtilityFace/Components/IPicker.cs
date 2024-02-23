using ACE.DatLoader.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilityFace.HUDs;

namespace UtilityFace.Components;
public abstract class IPicker<T> : IComp
{
    /// <summary>
    /// The result of a selection
    /// </summary>
    public T Selection;

    /// <summary>
    /// Label used for the component
    /// </summary>
    public string Name => $"{Label}###{_id}";
}

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

public abstract class IPagedPicker<T> : ICollectionPicker<T>  //where T
{
    //public uint Index;
    public int CurrentPage;
    public int PerPage = 20;

    public int Pages => Choices is null ? 0 : (int)(Choices.Length / PerPage) + 1;
    int offset => CurrentPage * PerPage;

    public virtual void DrawPageControls()
    {
        ImGui.SliderInt("Page", ref CurrentPage, 0, Pages, $"{CurrentPage}/{Pages}");
        ImGui.SameLine();
    }

    public override void DrawBody()
    {
        DrawPageControls();

        //Don't think arrays are LINQ optimized so not using those methods
        //https://stackoverflow.com/questions/26685234/are-linqs-skip-and-take-optimized-for-arrays-4-0-edition#26685395
        for (var i = 0; i < PerPage; i++)
        {
            var current = i + offset;
            var choice = Choices[current];

            DrawItem(choice, i);
        }
    }
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

