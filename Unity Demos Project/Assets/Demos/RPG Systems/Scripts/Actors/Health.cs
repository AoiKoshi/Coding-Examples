using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int healthPoints;
    [SerializeField]
    private int maxHealthPoints;

    private void Start()
    {
        healthPoints = maxHealthPoints;
    }

    public int GetHP()
    {
        return healthPoints;
    }

    public int GetMaxHP()
    {
        return maxHealthPoints;
    }

    public void IncreaseMaxHP(int value)
    {
        int percentageHealthFilled = healthPoints / maxHealthPoints;
        maxHealthPoints += value;
        healthPoints = maxHealthPoints * percentageHealthFilled;
    }

    public void DecreaseMaxHP(int value)
    {
        int percentageHealthFilled = healthPoints / maxHealthPoints;
        maxHealthPoints -= value;
        healthPoints = maxHealthPoints * percentageHealthFilled;
    }

    public void TakeDamage(int takenDamage)
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

    public virtual void DeathEvent()
    {

    }

    public void RecoverHealth(int addedHealth)
    {
        healthPoints += addedHealth;
        if (healthPoints > maxHealthPoints)
        {
            healthPoints = maxHealthPoints;
        }
    }
}
