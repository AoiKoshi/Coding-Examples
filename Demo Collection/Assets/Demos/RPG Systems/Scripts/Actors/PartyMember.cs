using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMember : Actor
{
    [Header("Graphics")]
    public Sprite smallAvatar;
    public Sprite fullBodyArtwork;

    [Header("Party")]
    public bool isActive;

    [Serializable]
    public class CharacterEquipment
    {
        [Header("Equipment")]
        public Weapon currentWeapon;
        [Range(0, 2)]
        public int numberOfAccessorySlots = 0;
        public Accessory currentAccessory;
        public Accessory currentAccessory2;
        public Armour currentArmour;

        public void ObtainAccessorySlot()
        {
            if (numberOfAccessorySlots < 2)
            {
                numberOfAccessorySlots++;
            }
        }
    }
    public CharacterEquipment equipment = new CharacterEquipment();

    public new PMStatSheet stats
    {
        get
        {
            return gameObject.GetComponent<PMStatSheet>();
        }
    }

    [Serializable]
    public class CharacterStatus
    {
        public enum statusEffectsTypes
        {
            poisoned,
            petrified,
            asleep,
            slowed,
            cursed
        }
        public List<statusEffectsTypes> statusEffects = new List<statusEffectsTypes>();

        public void AddStatusEffect(statusEffectsTypes statusEffect)
        {
            statusEffects.Add(statusEffect);
        }

        public void RemoveStatusEffect(statusEffectsTypes statusEffect)
        {
            statusEffects.Remove(statusEffect);
        }

        public bool HasStatusEffect(statusEffectsTypes statusEffect)
        {
            if (statusEffects.Contains(statusEffect))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public CharacterStatus status = new CharacterStatus();

    public MagicSheet magic
    {
        get
        {
            return gameObject.GetComponent<MagicSheet>();
        }
    }

    public SkillSheet skills
    {
        get
        {
            return gameObject.GetComponent<SkillSheet>();
        }
    }

    private void Awake()
    {
        stats.RecoverHealth(stats.GetMaxHP());
        stats.RecoverMP(stats.GetMaxMP());
    }
}
