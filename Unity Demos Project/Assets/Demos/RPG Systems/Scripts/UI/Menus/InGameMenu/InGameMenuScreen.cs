using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InGameMenuScreen : MonoBehaviour
{
    private RPGController playerController;

    [SerializeField] [Tooltip("Buttons that direct to a new sub-menu.")]
    private List<GameObject> navButtons = new List<GameObject>();

    [Serializable]
    public class Party
    {
        public PartyManager partyManager;
        [Tooltip("Party members will be displayed using this format.")]
        public GameObject panelTemplate;
        public GameObject contentContainer;
        public List<GameObject> panels = new List<GameObject>();
    }
    [SerializeField]
    private Party party = new Party();

    [Serializable]
    public class Menus
    {
        public GameObject inventory;
        public GameObject equipMenu;
    }
    [SerializeField]
    private Menus menus = new Menus();

    [SerializeField]
    private Button[] navigationButtons;

    [SerializeField]
    private Text contextText;
    public static int AccessLevel; //How deep into the menu is the player?
    private bool allowExit;

    private bool enableControls;

    public enum SubMenus
    {
        none,
        equip,
        inventory
    }
    public SubMenus subMenuOpened;

    public static InGameMenuScreen Instance { get; private set; }

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

        playerController = RPGController.Instance;
    }

    private void OnEnable()
    {
        GenPartyPanels();
        ResetNavigation();
    }

    public void OnToggleViewInventory(bool isDisplayed)
    {
        AccessLevel = 2;
        enableControls = false;
        menus.inventory.SetActive(isDisplayed);
    }

    public void OnToggleEquipMenu(bool isDisplayed)
    {
        AccessLevel = 2;
        enableControls = false;
        menus.equipMenu.SetActive(isDisplayed);
    }

    public void EngagePartyFormation()
    {
        party.panels[0].GetComponent<PartyMemberPanel>().components.button.Select();
        ToggleNavigationButtons(false);
        AccessLevel = 1;
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (subMenuOpened == SubMenus.none)
            {
                if (enableControls)
                {
                    if (AccessLevel == 0)
                    {
                        try
                        {
                            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                            button.onClick.Invoke();
                            ToggleNavigationButtons(false);
                            TogglePartyNavigation(false);
                        }
                        catch
                        {
                            //Play error sfx
                        }
                    }
                    else if (AccessLevel == 1) //Party formation
                    {
                        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                    }
                }
            }
        }
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if(enableControls)
            {
                if (AccessLevel == 0 && allowExit)
                {
                    playerController.TogglePauseState(false);
                    playerController.TriggerOverworldControls();
                    gameObject.SetActive(false);
                }
                else if (AccessLevel == 1)
                {
                    ResetNavigation();
                }
            }
        }
    }

    public void ResetNavigation()
    {
        ToggleNavigationButtons(true);
        TogglePartyNavigation(true);
        UpdatePartyPanels();
        allowExit = false;
        navButtons[0].GetComponent<Button>().Select();
        AccessLevel = 0;
        InGameMenuScreen.Instance.subMenuOpened = InGameMenuScreen.SubMenus.none;
        StartCoroutine(DelayAllowExit());
    }

    private void GenPartyPanels()
    {
        foreach(PartyMember i in party.partyManager.partyMembers)
        {
            GameObject button = Instantiate(party.panelTemplate);
            button.transform.SetParent(party.contentContainer.transform, false);
            button.GetComponent<PartyMemberPanel>().SetCharacter(i);
            party.panels.Add(button);
        }
    }

    public void ToggleNavigationButtons(bool toggle)
    {
        foreach (Button i in navigationButtons)
        {
            i.enabled = toggle;
        }
    }

    public void TogglePartyNavigation(bool toggle)
    {
        for(int i = 0; i < party.panels.Count; i++)
        {
            Button panelButton = party.panels[i].GetComponent<PartyMemberPanel>().components.button;
            panelButton.enabled = toggle;
        }
    }

    public void UpdateContext(string context)
    {
        contextText.text = context;
    }

    public void UpdatePartyPanels()
    {
        foreach (GameObject i in party.panels)
        {
            Destroy(i);
        }
        party.panels = new List<GameObject>();
        GenPartyPanels();
    }

    private void OnDisable()
    {
        foreach(GameObject i in party.panels)
        {
            Destroy(i);
        }
        party.panels = new List<GameObject>();
    }

    private IEnumerator DelayAllowExit()
    {
        yield return new WaitForSeconds(0.4f);
        allowExit = true;
        enableControls = true;
    }
}
