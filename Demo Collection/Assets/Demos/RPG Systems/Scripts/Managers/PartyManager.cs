using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField]
    private int activePartySlots;
    public List<PartyMember> partyMembers = new List<PartyMember>();
    public List<PartyMember> activeMembers = new List<PartyMember>();
    private PartyMember leadPartyMember;

    public static PartyManager Instance { get; private set; }

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
    }

    private void Start()
    {
        if(partyMembers.Count > 0)
        {
            MakeActive(partyMembers[0]);
            leadPartyMember = partyMembers[0];
        }
    }

    public void AddPartyMember(PartyMember character)
    {
        partyMembers.Add(character);
        MakeActive(character);
    }

    public void RemovePartyMember(PartyMember character)
    {
        partyMembers.Remove(character);
    }

    public void AlterActiveFormation(int position, PartyMember pMember)
    {
        if(activeMembers[position])
        {
            activeMembers[position].isActive = false;
        }
        activeMembers[position] = pMember;
        pMember.isActive = true;
    }

    public void ToggleActive(PartyMember pMember)
    {
        if(pMember.isActive)
        {
            if (pMember != leadPartyMember)
            {
                RemoveActive(pMember);
            }
        }
        else
        {
            MakeActive(pMember);
        }
    }

    public void MakeActive(PartyMember pMember)
    {
        if (activeMembers.Count < activePartySlots)
        {
            pMember.isActive = true;
            activeMembers.Add(pMember);
        }
    }

    public void RemoveActive(PartyMember pMember)
    {
        if(activeMembers.Count > 1)
        {
            pMember.isActive = false;
            activeMembers.Remove(pMember);
        }
    }

    public bool CheckIfInParty(PartyMember character)
    {
        return partyMembers.Contains(character);
    }
}
