using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStatSheet : MonoBehaviour
{
    public bool isInvincible;

    [Header("Status stats")]
    public float healthPoints;
    public float maxHealthPoints;
    public float manaPoints;
    public float maxManaPoints;

    [Header("Combat stats")]
    public float strength = 5;
    public float defence = 5;
    public float magic = 5;
    public float magicDefence = 5;
    public float agility = 5;
    public float luck = 5;
    public float evasion = 5;
    public float accuracy = 5;

    [Header("Resistances")]
    public float fireResistance;
    public float waterResistance;
    public float lightningResistance;
    public float iceResistance;
    public float darkResistance;
    public float divineResistance;
    public float poisonResistance;
    public float stoneResistance;
    public float slowResistance;
    public float sleepResistance;
    public float deathResistance;

    public void TakeDamage(float takenDamage)
    {
        if (!isInvincible)
        {
            healthPoints -= takenDamage;
            CheckDeath();
        }
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
}
