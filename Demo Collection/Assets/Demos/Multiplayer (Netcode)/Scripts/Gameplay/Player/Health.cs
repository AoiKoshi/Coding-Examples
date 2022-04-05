using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Threadwork.Gameplay.Player
{
    public class Health : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int maxHealthPoints;
        private int healthPoints;
        private int healthTarget;

        [Header("HUD")]
        [SerializeField] private Image healthBar;

        private void Start()
        {
            healthPoints = maxHealthPoints;
            UpdateHealthBar();
        }

        public int GetHP()
        {
            return healthPoints;
        }

        public int GetMaxHP()
        {
            return maxHealthPoints;
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

        public void TakeDamage(int takenDamage)
        {
            healthPoints -= takenDamage;
            UpdateHealthBar();
            CheckDeath();
        }

        public void RecoverHealth(int addedHealth)
        {
            healthPoints += addedHealth;
            if (healthPoints > maxHealthPoints)
            {
                healthPoints = maxHealthPoints;
            }
            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            healthTarget = healthPoints / maxHealthPoints;
            if (healthTarget > healthPoints)
            {
                StartCoroutine(TrickleHealthBar(true));
            }
            else
            {
                StartCoroutine(TrickleHealthBar(false));
            }
        }

        private IEnumerator TrickleHealthBar(bool increase)
        {
            while (healthPoints != healthTarget)
            {
                if (increase)
                {
                    healthPoints += Mathf.RoundToInt(Time.deltaTime);
                }
                else
                {
                    healthPoints -= Mathf.RoundToInt(Time.deltaTime);
                }
                healthBar.fillAmount = healthPoints / maxHealthPoints;
                yield return new WaitForEndOfFrame();
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

        public virtual void DeathEvent()
        {

        }
    }
}
