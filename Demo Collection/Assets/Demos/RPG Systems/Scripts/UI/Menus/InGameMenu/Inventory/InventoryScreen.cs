using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InventoryScreen : MonoBehaviour
{
    [SerializeField]
    private InventoryParty party;

    [SerializeField] [Tooltip("Items will be displayed using this format.")]
    private GameObject panelTemplate;
    [SerializeField] 
    private GameObject contentContainer;
    public InventoryInformationPanel informationPanel;
    [Space(10)]
    public List<GameObject> buttons = new List<GameObject>();
    private List<GameObject> items = new List<GameObject>();
    private int categoryIndex = 1;

    public Item currentItemSelected;
    public Restoratives currentRestorativeSelected;
    public InventoryPanel currentItemSelectedPanel;
    public PartyMember currentPartyMember;
    public InventoryPartyMemberPanel currentPartyMemberPanel;

    private InGameMenuScreen mainMenu;
    private RPGController playerController;
    private Inventory playerInventory;

    public bool allowNavigation { get; private set; }

    public static InventoryScreen Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        categoryIndex = 1;

        mainMenu = InGameMenuScreen.Instance;
        playerController = RPGController.Instance;
        playerInventory = PlayerInventory.Instance;
    }

    private void OnEnable()
    {
        playerInventory = GameObject.FindObjectOfType<Inventory>();
        ChangeCategory(categoryIndex);
        playerController.TriggerMenuControls();
        StartCoroutine(DelayAllowNavigation());
    }

    public void ChangeCategory(int i)
    {
        categoryIndex = i;
        switch (categoryIndex)
        {
            case 1:
                informationPanel.categoryHeader.text = "Items";
                break;
            case 2:
                informationPanel.categoryHeader.text = "Weapons";
                break;
            case 3:
                informationPanel.categoryHeader.text = "Accessories";
                break;
            case 4:
                informationPanel.categoryHeader.text = "Armour";
                break;
            case 5:
                informationPanel.categoryHeader.text = "Loot";
                break;
            case 6:
                informationPanel.categoryHeader.text = "Key Items";
                break;
        }

        ClearButtons();
        GenButtons();
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (InGameMenuScreen.Instance.subMenuOpened == InGameMenuScreen.SubMenus.inventory)
            {
                if (allowNavigation)
                {
                    if (InGameMenuScreen.AccessLevel == 2)
                    {
                        if (categoryIndex == 1)
                        {
                            InventoryPanel button = EventSystem.current.currentSelectedGameObject.GetComponent<InventoryPanel>();
                            button.OnHighlight();
                            party.partyOverview.SetActive(true);
                            GenPartyPanels();
                            TogglePartyNavigation(true);
                            ResetPartyNavigation();
                            InGameMenuScreen.AccessLevel = 3;
                        }
                    }

                    else if (InGameMenuScreen.AccessLevel == 3)
                    {
                        currentRestorativeSelected = currentItemSelected as Restoratives;
                        currentRestorativeSelected.Restore(currentPartyMember);
                        currentItemSelectedPanel.OnHighlight();
                        currentPartyMemberPanel.UpdatePanel();
                        ReturnToItemSelection();
                        currentPartyMemberPanel.components.button.Select();
                    }
                }
            }
        }
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if(gameObject.activeSelf)
            {
                if (InGameMenuScreen.AccessLevel == 3)
                {
                    party.partyOverview.SetActive(false);
                    TogglePartyNavigation(false);
                    ReturnToItemSelection();
                    ClearParty();
                    InGameMenuScreen.AccessLevel -= 1;
                }
                else if (InGameMenuScreen.AccessLevel == 2)
                {
                    ExitInventoryScreen();
                }
            }
        }
    }

    public void OnToggleLeft(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            ToggleCategories(true);
        }
    }

    public void OnToggleRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ToggleCategories(false);
        }
    }

    public void ToggleCategories(bool left)
    {
        if (left && categoryIndex > 1)
        {
            categoryIndex -= 1;
        }
        else if(!left && categoryIndex < 6)
        {
            categoryIndex += 1;
        }
        ChangeCategory(categoryIndex);
    }

    private void GenButtons()
    {
        foreach(Item i in playerInventory.items)
        {
            if(i.properties.type == categoryIndex)
            {
                GameObject button = Instantiate(panelTemplate);
                button.GetComponent<InventoryPanel>().SetItem(i);
                button.transform.SetParent(contentContainer.transform, false);
                items.Add(button);
            }
        }
        if(items.Count > 0)
        {
            items[0].GetComponent<InventoryPanel>().OnClick();
            items[0].GetComponent<Button>().Select();
            currentItemSelected = items[0].GetComponent<InventoryPanel>().GetStoredItem();
        }
    }

    public PartyMember GetCurrentPartyMember()
    {
        return currentPartyMember;
    }

    private void ResetPartyNavigation()
    {
        party.panels[0].GetComponent<InventoryPartyMemberPanel>().components.button.Select();
    }

    private void ReturnToItemSelection()
    {
        currentItemSelectedPanel.GetComponent<Button>().Select();
    }

    public void TogglePartyNavigation(bool toggle)
    {
        for (int i = 0; i < party.panels.Count; i++)
        {
            Button panelButton = party.panels[i].GetComponent<InventoryPartyMemberPanel>().components.button;
            panelButton.enabled = toggle;
        }
        party.partyOverview.SetActive(toggle);
    }

    private void GenPartyPanels()
    {
        foreach (PartyMember i in party.partyManager.partyMembers)
        {
            GameObject button = Instantiate(party.panelTemplate);
            button.transform.SetParent(party.contentContainer.transform, false);
            button.GetComponent<InventoryPartyMemberPanel>().SetCharacter(i);
            party.panels.Add(button);
        }

        currentPartyMemberPanel = party.panels[0].GetComponent<InventoryPartyMemberPanel>();
        currentPartyMember = party.panels[0].GetComponent<InventoryPartyMemberPanel>().character;
    }

    private void ClearButtons()
    {
        foreach(GameObject button in items)
        {
            Destroy(button);
        }
        items = new List<GameObject>();

        informationPanel.descriptionText.text = "";
        informationPanel.itemPicture.sprite = informationPanel.emptyPicture;
    }

    private void ClearParty()
    {
        foreach (GameObject i in party.panels)
        {
            Destroy(i);
        }
        party.panels = new List<GameObject>();
    }

    private void ExitInventoryScreen()
    {
        allowNavigation = false;
        mainMenu.ResetNavigation();
        gameObject.SetActive(false);
    }

    private IEnumerator DelayAllowNavigation()
    {
        yield return new WaitForSeconds(0.2f);
        allowNavigation = true;
        InGameMenuScreen.Instance.subMenuOpened = InGameMenuScreen.SubMenus.inventory;
    }
}
