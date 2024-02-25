using UtilityFace.HUDs;
using SpellBook = UtilityBelt.Scripting.Interop.SpellBook;

namespace UtilityFace.Components;

public class SpellLevelFilter : IOptionalFilter<SpellInfo>
{
    public bool[] Levels = Enumerable.Range(0, 8).Select(x => true).ToArray();
    static readonly SpellBook spellbook = HudBase.game.Character.SpellBook;

    public override void DrawBody()
    {
        for (var i = 0; i < Levels.Length; i++)
        {
            ImGui.SameLine();
            if (ImGui.Checkbox($"{i + 1}", ref Levels[i]))
                Changed = true;
        }
    }

    public override bool IsFiltered(SpellInfo item)
    {
        if (!spellbook.TryGet(item.Id, out var spellData))
            return false;

        var level = spellData.Level - 1;

        //if(item.Id == 1)
        //    Log.Chat($"{item.Spell.Name} - {level} - {Levels[level]}");
        
        //Check for invalid level
        if (level < 0 || level >= Levels.Length)
            return false;

        return !Levels[level];
    }
}
