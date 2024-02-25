using ACE.DatLoader.Entity;
using ACE.DatLoader.FileTypes;
using System.ComponentModel.Design;
using UtilityFace.HUDs;
using SpellBook = UtilityBelt.Scripting.Interop.SpellBook;

namespace UtilityFace.Components;
public class SpellFilter : IOptionalFilter<SpellInfo>
{
    static readonly SpellTable table = UBService.PortalDat.SpellTable;
    static readonly SpellBook spellbook = HudBase.game.Character.SpellBook;

    RegexFilter<SpellBase> nameFilter = new(x => x.Name) { Active = true, Label = "Name" };

    public bool UseKnown = false;

    public bool UseCastable = false;
    public float CastChance = 1f;

    public bool UseLevels = false;
    public bool[] Levels = Enumerable.Range(0, 8).Select(x => false).ToArray();

    //MagicSchool
    public bool UseSchools = false;
    public static readonly string[] SchoolNames = //Enum.GetNames(typeof(MagicSchool));
    {
       "War", "Life", "Item", "Creature", "Void"
    };
    public bool[] Schools = SchoolNames.Select(x => true).ToArray();

    public override void DrawBody()
    {
        ImGui.NewLine();
        if (nameFilter.Check())
            Changed = true;

        if (ImGui.Checkbox("Known", ref UseKnown))
            Changed = true;

        //ImGui.SameLine();
        if (ImGui.Checkbox("Success", ref UseCastable))
            Changed = true;

        if (UseCastable)
        {
            //ImGui.SameLine();
            if (ImGui.SliderFloat("Chance", ref CastChance, 0, 1, $"{CastChance:P2}%"))
                Changed = true;
        }

        if (ImGui.Checkbox("Schools", ref UseSchools))
            Changed = true;

        if (UseSchools)
        {
            for (var i = 0; i < SchoolNames.Length; i++)
            {
                ImGui.SameLine();
                if (ImGui.Checkbox($"{SchoolNames[i]}", ref Schools[i]))
                    Changed = true;
            }
        }

        if (ImGui.Checkbox("Levels", ref UseLevels))
            Changed = true;

        if (UseLevels)
        {
            for (var i = 0; i < Levels.Length; i++)
            {
                ImGui.SameLine();
                if (ImGui.Checkbox($"{i + 1}", ref Levels[i]))
                    Changed = true;
            }
        }
    }

    //public KeyValuePair<uint, SpellBase>[] GetOrdered()
    //{
    //    return GetFiltered();
    //    //return GetFiltered().OrderBy(x => x.)
    //}

    /// <summary>
    /// Returns a filtered list of spells using UBs list
    /// </summary>
    public SpellInfo[] GetFilteredSpells() => GetFiltered(table.Spells.Select(x => new SpellInfo(x.Key, x.Value))).ToArray();
        //public IEnumerable<KeyValuePair<uint, SpellBase>> GetFilteredSpells() => GetFiltered(table.Spells);

    public override bool IsFiltered(SpellInfo spellInfo)
    {
        FilterSet foo = new(new()
        {
            nameFilter
        });

        if (!Active)
            return false;

        var spell = spellInfo.Spell;
        var id = spellInfo.Id;

        //Add desc?
        if (nameFilter.Active && nameFilter.IsFiltered(spell))
            return true;

        if (UseSchools)
        {
            int num = (int)spell.School - 1;    //0 is Unknown
            if (num >= 0 && num < Schools.Length && !Schools[num])
                return true;
        }

        if (UseKnown && !spellbook.IsKnown(id))
            return true;

        if (UseCastable)
        {
            int skill = HudBase.game.Character.GetMagicSkill(spell.School);
            var chance = SkillCheck.GetMagicSkillChance(skill, (int)spell.Power) + .001f;

            if (chance < CastChance)
                return true;
        }

        //Anything requiring SpellBook data below here
        if (!spellbook.TryGet(id, out var spellData))
            return false;

        if (UseLevels)
        {
            var level = spellData.Level - 1;
            if (level >= 0 && level < Levels.Length && !Levels[level])
                return true;
        }

        return false;
    }
}

public record struct SpellInfo(uint Id, SpellBase Spell);