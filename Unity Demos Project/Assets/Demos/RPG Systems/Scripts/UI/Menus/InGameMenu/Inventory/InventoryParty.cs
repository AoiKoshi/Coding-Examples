using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryParty : MonoBehaviour
{
    public PartyManager partyManager;
    [Tooltip("Party members will be displayed using this format.")]
    public GameObject panelTemplate;
    public GameObject contentContainer;
    public List<GameObject> panels = new List<GameObject>();

    public GameObject partyOverview;
}
