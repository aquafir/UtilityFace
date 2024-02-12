using System.Text;
using UtilityBelt.Common.Messages.Events;
using UtilityFace.Enums;
using UtilityFace.HUDs;
using DamageType = UtilityFace.Enums.DamageType;

namespace UtilityFace.Helpers;
public static class DamageHelpers
{
    public static string GetMessage(this Combat_HandleAttackerNotificationEvent_S2C_EventArgs e)
    {
        return GetAttackMessage(e.Data.Name, (float)e.Data.DamagePercent, (Enums.DamageType)e.Data.DamageType, e.Data.DamageDone, e.Data.Conditions, e.Data.Critical);
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
            new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageEx.CombatAttackerNotification, false);
    public static ChatLog GetChatLog(this Combat_HandleDefenderNotificationEvent_S2C_EventArgs e) =>
    new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageEx.CombatDefenderNotification, false);
    public static ChatLog GetChatLog(this Combat_HandleEvasionAttackerNotificationEvent_S2C_EventArgs e) =>
    new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageEx.CombatEvasionAttackNotification, false);
    public static ChatLog GetChatLog(this Combat_HandleEvasionDefenderNotificationEvent_S2C_EventArgs e) =>
        new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageEx.CombatEvasionDefenderNotification, false);
    public static ChatLog GetChatLog(this Combat_HandleVictimNotificationEventSelf_S2C_EventArgs e) =>
    new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageEx.CombatVictimSelf, false);
    public static ChatLog GetChatLog(this Combat_HandleVictimNotificationEventOther_S2C_EventArgs e) =>
        new(0, null, e.GetMessage(), ChatChannel.None, ChatMessageEx.CombatVictimOther, false);


    public static string GetAttackMessage(this WorldObject source, WorldObject target, DamageType damageType, uint amount, AttackConditionsMask conditions = 0, bool isCrit = false)
    {
        int health = 1;
        if (target.Vitals.TryGetValue(VitalId.Health, out var hp))
            health = hp.Base;
        var percent = (float)amount / health;

        return GetAttackMessage(target.Name, percent, damageType, amount, conditions, isCrit);
    }

    public static string GetAttackMessage(string name, float percent, DamageType damageType, uint amount, AttackConditionsMask conditions = 0, bool isCrit = false)
    {
        string verb = null, plural = null;
        Strings.GetAttackVerb(damageType, percent, ref verb, ref plural);
        var type = damageType.GetName().ToLower();

        //Log.Chat($"{conditions}");

        //Crit protection aug condition?
        var prefix = $"{(isCrit ? "Critical hit! " : "")}{(conditions.HasFlag(AttackConditionsMask.SneakAttack) ? "Sneak attack! " : "")}{(conditions.HasFlag(AttackConditionsMask.Recklessness) ? "Recklessness! " : "")}";

        return $"{prefix}You {verb} {name} for {amount} points of {type} damage!";
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
