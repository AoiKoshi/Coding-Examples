using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restoratives : Item
{
    [SerializeField] [Range(0,100)]
    private int percentageToRestore;

    public enum restorativeTypes
    {
        healthRestore,
        manaRestore,
        poisonRestore,
        petrificationRestore,
        slownessRestore,
        asleepRestore,
        cursedRestore,
        allStatusEffects
    }
    [SerializeField]
    private restorativeTypes restorativeType;

    public override void Restore(PartyMember character)
    {
        bool isUsed = false;
        switch (restorativeType)
        {
            case restorativeTypes.healthRestore:
                if(!character.stats.IsFullyHealed())
                {
                    float amountToHeal = character.stats.GetMaxHP() * (percentageToRestore / 100f);
                    character.stats.RecoverHealth(amountToHeal);
                    isUsed = true;
                }
                break;
            case restorativeTypes.manaRestore:
                if (!character.stats.IsManaFull())
                {
                    float amountToRecover = character.stats.GetMaxMP() * (percentageToRestore / 100f);
                    character.stats.RecoverMP(amountToRecover);
                    isUsed = true;
                }
                break;
            case restorativeTypes.poisonRestore:
                if (character.status.HasStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.poisoned))
                {
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.poisoned);
                    isUsed = true;
                }
                break;
            case restorativeTypes.petrificationRestore:
                if (character.status.HasStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.petrified))
                {
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.petrified);
                    isUsed = true;
                }
                break;
            case restorativeTypes.slownessRestore:
                if (character.status.HasStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.slowed))
                {
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.slowed);
                    isUsed = true;
                }
                break;
            case restorativeTypes.asleepRestore:
                if (character.status.HasStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.asleep))
                {
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.asleep);
                    isUsed = true;
                }
                break;
            case restorativeTypes.cursedRestore:
                if (character.status.HasStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.cursed))
                {
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.cursed);
                    isUsed = true;
                }
                break;
            case restorativeTypes.allStatusEffects:
                if (character.status.statusEffects.Count > 0)
                {
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.poisoned);
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.petrified);
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.slowed);
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.asleep);
                    character.status.RemoveStatusEffect(PartyMember.CharacterStatus.statusEffectsTypes.cursed);
                    isUsed = true;
                }
                break;
        }
        if(isUsed)
        {
            GameObject.FindObjectOfType<Inventory>().RemoveFromInventory(this);
        }
    }
}
