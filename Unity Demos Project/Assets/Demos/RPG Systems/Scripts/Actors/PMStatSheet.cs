using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PartyMember))]
public class PMStatSheet : MonoBehaviour
{
    [Header("Progression stats")]
    private float experiencePoints;
    private float experienceTillNextLevel = 100;
    private float pathwayPoints;

    [Header("Status stats")]
    private float healthPoints;
    [SerializeField]
    private float maxHealthPoints;
    private float manaPoints;
    [SerializeField]
    private float maxManaPoints;

    [Header("Combat stats")]
    private float strength = 5;
    private float defence = 5;
    private float magic = 5;
    private float magicDefence = 5;
    private float agility = 5;
    private float luck = 5;
    private float evasion = 5;
    private float accuracy = 5;

    [Header("Multipliers")]
    private float healthBonus = 1;
    private float manaBonus = 1;
    private float strengthBonus = 1;
    private float defenceBonus = 1;
    private float magicBonus = 1;
    private float magicDefenceBonus = 1;
    private float agilityBonus = 1;
    private float luckBonus = 1;
    private float evasionBonus = 1;
    private float accuracyBonus = 1;

    [Header("Resistances")]
    private float fireResistance;
    private float waterResistance;
    private float lightningResistance;
    private float iceResistance;
    private float darkResistance;
    private float divineResistance;
    private float poisonResistance;
    private float stoneResistance;
    private float slowResistance;
    private float sleepResistance;
    private float deathResistance;

    public float GetHP()
    {
        return healthPoints;
    }

    public bool IsFullyHealed()
    {
        if (healthPoints == maxHealthPoints)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetMaxHP()
    {
        return maxHealthPoints * healthBonus;
    }

    public bool IsManaFull()
    {
        if (manaPoints == maxManaPoints)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void IncreaseMaxHP(float value)
    {
        float percentageHealthFilled = healthPoints / maxHealthPoints;
        maxHealthPoints = Mathf.Round(maxHealthPoints + value);
        healthPoints = Mathf.Round(maxHealthPoints * percentageHealthFilled);
    }

    public void DecreaseMaxHP(float value)
    {
        float percentageHealthFilled = healthPoints / maxHealthPoints;
        maxHealthPoints = Mathf.Round(maxHealthPoints - value);
        healthPoints = Mathf.Round(maxHealthPoints * percentageHealthFilled);
    }

    public void TakeDamage(float takenDamage)
    {
        healthPoints -= takenDamage;
        CheckDeath();
    }

    public void CheckDeath()
    {
        if (healthPoints < 0)
        {
            healthPoints = 0;
        }
        if (healthPoints == 0)
        {
            DeathEvent();
        }
    }

    public void DeathEvent()
    {

    }

    public void RecoverHealth(float addedHealth)
    {
        healthPoints += Mathf.Round(addedHealth);
        if (healthPoints > maxHealthPoints)
        {
            healthPoints = maxHealthPoints;
        }
    }

    public void RecoverMP(float addedMP)
    {
        manaPoints += Mathf.Round(addedMP);
        if (manaPoints > maxManaPoints)
        {
            manaPoints = maxManaPoints;
        }
    }

    public float GetXP()
    {
        return experiencePoints;
    }

    public float GetXPTillNextLevel()
    {
        return experienceTillNextLevel;
    }

    public float GetPathwayPoints()
    {
        return pathwayPoints;
    }

    public void AddXP(float value)
    {
        if (pathwayPoints < 99)
        {
            experiencePoints += Mathf.Round(value);
            if (experiencePoints >= experienceTillNextLevel)
            {
                pathwayPoints += 1;
                float leftoverXP = experienceTillNextLevel - experiencePoints;
                experiencePoints = Mathf.Round(leftoverXP);
            }
        }
    }

    public bool AttemptPathwayProgression()
    {
        if (pathwayPoints > 0)
        {
            pathwayPoints -= 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetMP()
    {
        return manaPoints;
    }

    public float GetMaxMP()
    {
        return maxManaPoints * manaBonus;
    }

    public void IncreaseMaxMP(float value)
    {
        float percentageManaFilled = manaPoints / maxManaPoints;
        maxManaPoints = Mathf.Round(maxManaPoints + value);
        manaPoints = Mathf.Round(maxManaPoints * percentageManaFilled);
    }

    public void DecreaseMaxMP(float value)
    {
        float percentageManaFilled = manaPoints / maxManaPoints;
        maxManaPoints = Mathf.Round(maxManaPoints - value);
        manaPoints = Mathf.Round(maxManaPoints * percentageManaFilled);
    }

    public float GetStat(string stat)
    {
        switch (stat.ToLower())
        {
            case "strength":
                return strength * strengthBonus;
            case "defence":
                return defence * defenceBonus;
            case "magic":
                return magic * magicBonus;
            case "magicdefence":
                return magicDefence * magicDefenceBonus;
            case "agility":
                return agility * agilityBonus;
            case "luck":
                return luck * luckBonus;
            case "evasion":
                return evasion * evasionBonus;
            case "accuracy":
                return accuracy * accuracyBonus;
            case "fireresistance":
                return fireResistance;
            case "waterresistance":
                return waterResistance;
            case "lightningresistance":
                return lightningResistance;
            case "iceresistance":
                return iceResistance;
            case "darkresistance":
                return darkResistance;
            case "divineresistance":
                return divineResistance;
            case "poisonresistance":
                return poisonResistance;
            case "stoneresistance":
                return stoneResistance;
            case "slowresistance":
                return slowResistance;
            case "sleepresistance":
                return sleepResistance;
            case "deathresistance":
                return deathResistance;
        }
        return 0;
    }

    public void SetStat(string stat, float value)
    {
        value = Mathf.Round(value);
        switch (stat.ToLower())
        {
            case "strength":
                strength = value;
                break;
            case "defence":
                defence = value;
                break;
            case "magic":
                magic = value;
                break;
            case "magicdefence":
                magicDefence = value;
                break;
            case "agility":
                agility = value;
                break;
            case "luck":
                luck = value;
                break;
            case "evasion":
                evasion = value;
                break;
            case "accuracy":
                accuracy = value;
                break;
            case "fireresistance":
                fireResistance = value;
                break;
            case "waterresistance":
                waterResistance = value;
                break;
            case "lightningresistance":
                lightningResistance = value;
                break;
            case "iceresistance":
                iceResistance = value;
                break;
            case "darkresistance":
                darkResistance = value;
                break;
            case "divineresistance":
                divineResistance = value;
                break;
            case "poisonresistance":
                poisonResistance = value;
                break;
            case "stoneresistance":
                stoneResistance = value;
                break;
            case "slowresistance":
                slowResistance = value;
                break;
            case "sleepresistance":
                sleepResistance = value;
                break;
            case "deathresistance":
                deathResistance = value;
                break;
        }
    }

    public float GetStatBonus(string stat)
    {
        switch (stat.ToLower())
        {
            case "health":
                return healthBonus;
            case "mana":
                return manaBonus;
            case "strength":
                return strengthBonus;
            case "defence":
                return defenceBonus;
            case "magic":
                return magicBonus;
            case "magicdefence":
                return magicDefenceBonus;
            case "agility":
                return agilityBonus;
            case "luck":
                return luckBonus;
            case "evasion":
                return evasionBonus;
            case "accuracy":
                return accuracyBonus;
        }
        return 0;
    }

    public void SetStatBonus(string stat, float value)
    {
        switch (stat.ToLower())
        {
            case "health":
                healthBonus = value;
                break;
            case "mana":
                manaBonus = value;
                break;
            case "strength":
                strengthBonus = value;
                break;
            case "defence":
                defenceBonus = value;
                break;
            case "magic":
                magicBonus = value;
                break;
            case "magicdefence":
                magicDefenceBonus = value;
                break;
            case "agility":
                agilityBonus = value;
                break;
            case "luck":
                luckBonus = value;
                break;
            case "evasion":
                evasionBonus = value;
                break;
            case "accuracy":
                accuracyBonus = value;
                break;
        }
    }
}
