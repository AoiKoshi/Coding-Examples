using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PartyDialogue : MonoBehaviour
{
    private RPGController player;
    private PartyManager partyManager;
    private PartyMember character;
    private bool removePartyMember;

    [SerializeField]
    private Text noticeText;

    [SerializeField]
    private GameObject proceedButton;
    private bool canProceed;

    private void OnEnable()
    {
        player = RPGController.Instance;
        partyManager = PartyManager.Instance;

        player.ToggleDialogueState(true);
        canProceed = false;
        proceedButton.SetActive(false);
        StartCoroutine(DelayConfirmation());
        StartCoroutine(HapticsManager.Instance.Popup());
    }

    public void SetCharacter(PartyMember character, bool removeFromParty)
    {
        this.character = character;
        removePartyMember = removeFromParty;
        if(removePartyMember)
        {
            noticeText.text = this.character.profile.firstName + " has left the party.";
        }
        else
        {
            noticeText.text = this.character.profile.firstName + " has joined the party.";
        }
    }

    public void OnCloseScreen()
    {
        if (removePartyMember)
        {
            PartyManager.Instance.RemovePartyMember(character);
        }
        else
        {
            PartyManager.Instance.AddPartyMember(character);
        }

        player.ToggleDialogueState(false);
        character.enabled = false;
        canProceed = false;
        gameObject.SetActive(false);
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (canProceed)
            {
                OnCloseScreen();
            }
        }
    }

    private IEnumerator DelayConfirmation()
    {
        yield return new WaitForSeconds(0.4f);
        canProceed = true;
        proceedButton.SetActive(true);
    }
}
