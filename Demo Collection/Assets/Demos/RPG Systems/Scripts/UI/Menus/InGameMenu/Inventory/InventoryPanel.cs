using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    private InventoryScreen inventoryScreen;

    [Serializable]
    public class PanelComponents
    {
        public Text nameHeader;
        public Text quantityText;
        public Image icon;
    }
    [SerializeField]
    private PanelComponents components = new PanelComponents();

    private Item itemStored;

    private void OnEnable()
    {
        inventoryScreen = InventoryScreen.Instance;
    }

    public void SetPanel(Item item)
    {
        components.nameHeader.text = item.properties.name;
        components.quantityText.text = "x" + item.properties.quantity;

        //Change icon by pulling icon from inventoryScreen's icon collections
    }

    public void OnClick()
    {
        
    }

    public void OnHighlight()
    {
        inventoryScreen.informationPanel.descriptionText.text = itemStored.properties.description;
        inventoryScreen.informationPanel.itemPicture.sprite = itemStored.graphics.overview;
        components.quantityText.text = "x" + itemStored.properties.quantity;
        inventoryScreen.currentItemSelectedPanel = this;
        inventoryScreen.currentItemSelected = itemStored;
    }

    public void SetItem(Item incomingItem)
    {
        itemStored = incomingItem;
        SetPanel(itemStored);
    }

    public Item GetStoredItem()
    {
        return itemStored;
    }
}
