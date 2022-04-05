using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class EquipOverview : MonoBehaviour
{
    private PartyMember currentCharacter;

    [Serializable]
    public class PanelComponents
    {
        public Text nameHeader;
        public Text healthText;
        public Text manaText;
        public Text xpLeftTillPP;
        public Text ppLevelText;
        public Image ppBar;
        public Image focusBar;
        public Image healthBar;
        public Image manaBar;
        public Image icon;

        public Text weaponName;
        public Text armourName;
        public Text accessoryName;
        public Button accessoryButton;
        public Text accessory2Name;
        public Button accessory2Button;
    }
    public PanelComponents components = new PanelComponents();

    private void OnEnable()
    {
        currentCharacter = EquipMenu.Instance.GetCurrentPartyMember();
        SetPanel(currentCharacter);
    }

    private void SetPanel(PartyMember character)
    {
        components.nameHeader.text = currentCharacter.profile.firstName + " " + currentCharacter.profile.lastName;
        components.healthText.text = currentCharacter.stats.GetHP() + "/" + currentCharacter.stats.GetMaxHP();
        components.manaText.text = currentCharacter.stats.GetMP() + "/" + currentCharacter.stats.GetMaxMP();
        components.xpLeftTillPP.text = currentCharacter.stats.GetXPTillNextLevel() + " XP until next Progression Point";
        components.ppLevelText.text = currentCharacter.stats.GetPathwayPoints().ToString();
        components.ppBar.fillAmount = currentCharacter.stats.GetXP() / currentCharacter.stats.GetXPTillNextLevel();
        components.healthBar.fillAmount = currentCharacter.stats.GetHP() / currentCharacter.stats.GetMaxHP();
        components.manaBar.fillAmount = currentCharacter.stats.GetMP() / currentCharacter.stats.GetMaxMP();
        components.icon.sprite = currentCharacter.smallAvatar;

        try
        {
            components.weaponName.text = currentCharacter.equipment.currentWeapon.properties.name;
        }
        catch
        {
            components.weaponName.text = "Empty";
        }

        try
        {
            components.armourName.text = currentCharacter.equipment.currentArmour.properties.name;
        }
        catch
        {
            components.armourName.text = "Empty";
        }

        
        if (currentCharacter.equipment.numberOfAccessorySlots == 1)
        {
            components.accessoryButton.interactable = true;
            components.accessory2Button.interactable = false;
        }
        else if(currentCharacter.equipment.numberOfAccessorySlots == 2)
        {
            components.accessoryButton.interactable = true;
            components.accessory2Button.interactable = true;
        }
        else
        {
            components.accessoryButton.interactable = false;
            components.accessory2Button.interactable = false;
        }

        //Accessory1
        try
        {
            components.accessoryName.text = currentCharacter.equipment.currentAccessory.properties.name;
        }
        catch
        {
            components.accessoryName.text = "Empty";
        }

        //Accessory2
        try
        {
            components.accessory2Name.text = currentCharacter.equipment.currentAccessory2.properties.name;
        }
        catch
        {
            components.accessory2Name.text = "Empty";
        }
    }
}
