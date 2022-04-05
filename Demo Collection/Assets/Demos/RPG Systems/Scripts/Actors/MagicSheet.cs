using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class MagicSheet : MonoBehaviour
{
    public List<Magic> spells = new List<Magic>();

    public void UnlockMagic(Magic magic)
    {
        spells.Add(magic);
    }

    public bool IsMagicUnlocked(Magic magic)
    {
        return spells.Contains(magic);
    }
}
