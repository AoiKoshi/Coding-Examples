using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class EquipMenu : MonoBehaviour
{
    [Serializable]
    public class Party
    {
        [Tooltip("Party members will be displayed using this format.")]
        public GameObject panelTemplate;
        public GameObject contentContainer;
        public List<GameObject> panels = new List<GameObject>();
    }
    [SerializeField]
    private Party party = new Party();

    [Serializable]
    public class Components
    {
        public GameObject partyPanel;
    }
    [SerializeField]
    private Components components = new Components();

    private PartyMember currentPartyMember;

    private InGameMenuScreen mainMenu;
    [SerializeField]
    private GameObject overview;

    public bool allowNavigation { get; private set; }

    public static EquipMenu Instance { get; private set; }

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

        mainMenu = InGameMenuScreen.Instance;
    }

    private void OnEnable()
    {
        GenPartyPanels();
        ResetNavigation();
        StartCoroutine(DelayAllowNavigation());

        party.panels[0].GetComponent<EquipPartyMemberPanel>().components.button.Select();
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (InGameMenuScreen.Instance.subMenuOpened == InGameMenuScreen.SubMenus.equip)
            {
                if (allowNavigation)
                {
                    if (InGameMenuScreen.AccessLevel == 2)
                    {
                        try
                        {
                            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                            SelectCharacter(button.transform.parent.GetComponent<EquipPartyMemberPanel>().character);
                        }
                        catch
                        {
                            //SFX
                        }
                    }
                }
            }
        }
    }

    public void SelectCharacter(PartyMember character)
    {
        currentPartyMember = character;
        TogglePartyNavigation(false);
        overview.SetActive(true);
        InGameMenuScreen.AccessLevel = 3;
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if(gameObject.activeSelf)
            {
                if (InGameMenuScreen.AccessLevel == 4)
                {
                    InGameMenuScreen.AccessLevel = 3;
                }
                else if (InGameMenuScreen.AccessLevel == 3)
                {
                    overview.SetActive(false);
                    TogglePartyNavigation(true);
                    ResetNavigation();
                    InGameMenuScreen.AccessLevel = 2;
                }
                else if (InGameMenuScreen.AccessLevel == 2)
                {
                    ExitEquipMenu();
                }
            }
        }
    }

    private void GenPartyPanels()
    {
        foreach (PartyMember i in PartyManager.Instance.partyMembers)
        {
            GameObject button = Instantiate(party.panelTemplate);
            button.transform.SetParent(party.contentContainer.transform, false);
            button.GetComponent<EquipPartyMemberPanel>().SetCharacter(i);
            party.panels.Add(button);
        }
    }

    private void ResetNavigation()
    {
        party.panels[0].GetComponent<EquipPartyMemberPanel>().components.button.Select();
    }

    public void TogglePartyNavigation(bool toggle)
    {
        for (int i = 0; i < party.panels.Count; i++)
        {
            Button panelButton = party.panels[i].GetComponent<EquipPartyMemberPanel>().components.button;
            panelButton.enabled = toggle;
        }
        components.partyPanel.SetActive(toggle);
    }

    public PartyMember GetCurrentPartyMember()
    {
        return currentPartyMember;
    }

    private void ExitEquipMenu()
    {
        allowNavigation = false;
        mainMenu.ResetNavigation();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (GameObject i in party.panels)
        {
            Destroy(i);
        }
        party.panels = new List<GameObject>();
    }

    private IEnumerator DelayAllowNavigation()
    {
        yield return new WaitForSeconds(0.2f);
        allowNavigation = true;
        InGameMenuScreen.Instance.subMenuOpened = InGameMenuScreen.SubMenus.equip;
    }
}
