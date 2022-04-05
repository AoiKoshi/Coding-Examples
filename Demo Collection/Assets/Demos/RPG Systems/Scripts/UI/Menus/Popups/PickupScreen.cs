using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PickupScreen : MonoBehaviour
{
    private RPGController player;
    private Inventory inventory;

    private Item currentItem;
    [SerializeField]
    private Text itemNameHeader;

    [SerializeField]
    private GameObject proceedButton;
    private bool canProceed;

    private void OnEnable()
    {
        player = RPGController.Instance;
        inventory = PlayerInventory.Instance;

        itemNameHeader.text = currentItem.properties.name + " x" + currentItem.properties.quantity;

        player.ToggleDialogueState(true);
        canProceed = false;
        proceedButton.SetActive(false);
        StartCoroutine(DelayConfirmation());
        StartCoroutine(HapticsManager.Instance.Popup());
    }

    public void SetCurrentItem(Item item)
    {
        currentItem = item;
    }

    public void AddToInventory()
    {
        inventory.AddToInventory(currentItem);
        currentItem.gameObject.SetActive(false);
        OnCloseScreen();
    }

    public void OnCloseScreen()
    {
        player.ToggleDialogueState(false);
        canProceed = false;
        gameObject.SetActive(false);
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (canProceed)
            {
                AddToInventory();
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
