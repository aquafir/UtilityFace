using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Common.Messages.Events;
using UtilityFace.HUDs;

namespace UtilityFace.Helpers;
public static class DamageHelpers
{
    public static string GetMessage(this Combat_HandleAttackerNotificationEvent_S2C_EventArgs e)
    {
        return "";
    }
    public static string GetMessage(this Combat_HandleDefenderNotificationEvent_S2C_EventArgs e)
    {
        return "";
    }
    public static string GetMessage(this Combat_HandleEvasionAttackerNotificationEvent_S2C_EventArgs e) => $"{e.Data.Name} has evaded your attack.";
    public static string GetMessage(this Combat_HandleEvasionDefenderNotificationEvent_S2C_EventArgs e) => $"You evaded {e.Data.Name}.";
    public static string GetMessage(this Combat_HandleVictimNotificationEventSelf_S2C_EventArgs e) => $"{e.Data.Message}";
    public static string GetMessage(this Combat_HandleVictimNotificationEventOther_S2C_EventArgs e) => $"{e.Data.Message}";


    public static ChatLog GetChatLog(this Combat_HandleAttackerNotificationEvent_S2C_EventArgs e) =>
    new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageType.Combat, false);
    public static ChatLog GetChatLog(this Combat_HandleDefenderNotificationEvent_S2C_EventArgs e) =>
    new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageType.Combat, false);
    public static ChatLog GetChatLog(this Combat_HandleEvasionAttackerNotificationEvent_S2C_EventArgs e) =>
    new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageType.CombatEnemy, false);
    public static ChatLog GetChatLog(this Combat_HandleEvasionDefenderNotificationEvent_S2C_EventArgs e) =>
        new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageType.CombatSelf, false);
    public static ChatLog GetChatLog(this Combat_HandleVictimNotificationEventSelf_S2C_EventArgs e) =>
    new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageType.Combat, false);
    public static ChatLog GetChatLog(this Combat_HandleVictimNotificationEventOther_S2C_EventArgs e) =>
        new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageType.Combat, false);


    //public virtual string GetAttackMessage(Creature creature, DamageType damageType, uint amount)
    public static string GetAttackMessage(this WorldObject source, WorldObject target, DamageType damageType, uint amount)
    {
        int health = 1;
        if (target.Vitals.TryGetValue(VitalId.Health, out var hp))
            health = hp.Base;
        var percent = (float)amount / health;
        string verb = null, plural = null;
        Strings.GetAttackVerb(damageType, percent, ref verb, ref plural);
        var type = damageType.GetName().ToLower();
        return $"You {verb} {target.Name} for {amount} points of {type} damage!";
    }

    public static string GetName(this DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Undef: return "Undefined";
            case DamageType.Slash: return "Slashing";
            case DamageType.Pierce: return "Piercing";
            case DamageType.Bludgeon: return "Bludgeoning";
            case DamageType.Cold: return "Cold";
            case DamageType.Fire: return "Fire";
            case DamageType.Acid: return "Acid";
            case DamageType.Electric: return "Electric";
            case DamageType.Health: return "Health";
            case DamageType.Stamina: return "Stamina";
            case DamageType.Mana: return "Mana";
            case DamageType.Nether: return "Nether";
            case DamageType.Base: return "Base";
            default:
                return null;
        }
    }

    public static bool IsMultiDamage(this DamageType damageType)
    {
        return EnumHelper.HasMultiple((uint)damageType);
    }

    public static DamageType SelectDamageType(this DamageType damageType, float? powerLevel = null)
    {
        if (powerLevel == null)
        {
            // select random damage type
            var damageTypes = EnumHelper.GetFlags(damageType);

            return (DamageType)damageTypes.Random();
        }

        var playerTypes = powerLevel < 0.33f ? damageType & DamageType.Physical : damageType & ~DamageType.Physical;

        if (playerTypes == DamageType.Undef)
            playerTypes = damageType;

        return playerTypes.SelectDamageType();
    }

}
[Flags]
public enum DamageType
{
    Undef = 0x0,
    Slash = 0x1,
    Pierce = 0x2,
    Bludgeon = 0x4,
    Cold = 0x8,
    Fire = 0x10,
    Acid = 0x20,
    Electric = 0x40,
    Health = 0x80,
    Stamina = 0x100,
    Mana = 0x200,
    Nether = 0x400,
    Base = 0x10000000,

    // helpers
    Physical = Slash | Pierce | Bludgeon,
    Elemental = Cold | Fire | Acid | Electric,
};
