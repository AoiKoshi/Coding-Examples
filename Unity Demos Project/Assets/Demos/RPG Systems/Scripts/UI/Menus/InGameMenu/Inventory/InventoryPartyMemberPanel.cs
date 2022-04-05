using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryPartyMemberPanel : MonoBehaviour
{
    [Serializable]
    public class PanelComponents
    {
        public Text nameHeader;
        public Text healthText;
        public Image healthBar;
        public Text manaText;
        public Image manaBar;
        public Button button;
    }
    public PanelComponents components = new PanelComponents();

    public PartyMember character;
    private InventoryScreen inventoryScreen;

    private void OnEnable()
    {
        inventoryScreen = InventoryScreen.Instance;
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
        components.healthBar.fillAmount = this.character.stats.GetHP() / this.character.stats.GetMaxHP();
        components.manaText.text = this.character.stats.GetMP() + "/" + this.character.stats.GetMaxMP();
        components.manaBar.fillAmount = this.character.stats.GetMP() / this.character.stats.GetMaxMP();
    }

    public void OnHighlight()
    {
        inventoryScreen.currentPartyMemberPanel = this;
        inventoryScreen.currentPartyMember = character;
    }

    public void UpdatePanel()
    {
        SetPanel(character);
    }

    public void OnClick()
    {

    }
}
