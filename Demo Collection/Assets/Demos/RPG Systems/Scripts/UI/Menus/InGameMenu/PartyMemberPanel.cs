using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PartyMemberPanel : MonoBehaviour
{
    [Serializable]
    public class PanelComponents
    {
        public Text nameHeader;
        public Text healthText;
        public Text pptext;
        public Image xpDial;
        public Image healthBar;
        public Image manaBar;
        public Image activeIcon;
        public Button button;
        public Image icon;
    }
    public PanelComponents components = new PanelComponents();

    private PartyMember character;
    private PartyManager partyManager;

    private void Start()
    {
        partyManager = PartyManager.Instance;
    }

    public void SetCharacter(PartyMember character)
    {
        this.character = character;
        SetPanel(this.character);
    }

    public void SetPanel(PartyMember character)
    {
        components.nameHeader.text = this.character.profile.firstName;
        components.healthText.text = this.character.stats.GetHP() + "/" + this.character.stats.GetMaxHP();
        components.pptext.text = this.character.stats.GetPathwayPoints().ToString();
        components.xpDial.fillAmount = this.character.stats.GetXP() / this.character.stats.GetXPTillNextLevel();
        components.healthBar.fillAmount = this.character.stats.GetHP() / this.character.stats.GetMaxHP();
        components.manaBar.fillAmount = this.character.stats.GetMP() / this.character.stats.GetMaxMP();
        components.icon.sprite = this.character.smallAvatar;

        components.activeIcon.gameObject.SetActive(this.character.isActive);
    }

    public void OnClick()
    {
        partyManager.ToggleActive(character);
        components.activeIcon.gameObject.SetActive(character.isActive);
    }
}
